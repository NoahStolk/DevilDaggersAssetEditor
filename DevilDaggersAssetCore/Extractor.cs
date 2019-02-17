using DevilDaggersAssetCore.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevilDaggersAssetCore
{
	public class Extractor
	{
		public const ushort CHUNK_MODEL = 0x01;
		public const ushort CHUNK_TEXTURE = 0x02;
		public const ushort CHUNK_SHADER_VERTEX = 0x10;
		public const ushort CHUNK_SHADER_FRAGMENT = 0x11;
		public const ushort CHUNK_AUDIO = 0x20;
		public const ushort CHUNK_MODEL_BINDING = 0x80;

		public ulong MAGIC_1;
		public ulong MAGIC_2;

		public static ulong MAKE_MAGIC(ulong a, ulong b, ulong c, ulong d)
		{
			return a | b << 8 | c << 16 | d << 24;
		}

		public List<AbstractChunk> Chunks { get; set; } = new List<AbstractChunk>();

		public Extractor()
		{
			MAGIC_1 = MAKE_MAGIC(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
			MAGIC_2 = MAKE_MAGIC(0x72UL, 0x67UL, 0x3AUL, 0x01UL);
		}

		public void Extract(string inputPath, string outputPath)
		{
			Directory.CreateDirectory(outputPath);

			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			FileHeader archiveHeader = new FileHeader(
				magicNumber1: BitConverter.ToUInt32(sourceFileBytes, 0),
				magicNumber2: BitConverter.ToUInt32(sourceFileBytes, 4),
				tocSize: BitConverter.ToUInt32(sourceFileBytes, 8)
			);

			if (archiveHeader.MagicNumber1 != MAGIC_1 && archiveHeader.MagicNumber2 != MAGIC_2)
			{
				throw new Exception($"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {archiveHeader.MagicNumber1} should be {MAGIC_1}\nHeader value 2: {archiveHeader.MagicNumber2} should be {MAGIC_2}");
			}

			byte[] tocBuffer = new byte[archiveHeader.TocSize];
			Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)archiveHeader.TocSize);

			// Read toc and create chunks
			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe...
			{
				ushort type = tocBuffer[i];
				StringBuilder name = new StringBuilder();
				int nameLen = 0;
				for (; ; )
				{
					nameLen++;
					char c = (char)tocBuffer[i + nameLen + 1];
					if (c == '\0')
						break;
					name.Append(c);
				}
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + nameLen + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + nameLen + 6);
				uint unknown = BitConverter.ToUInt32(tocBuffer, i + nameLen + 10);

				AbstractChunk chunk;
				switch (type)
				{
					case CHUNK_AUDIO:
						chunk = new AudioChunk(name.ToString(), startOffset, size, unknown);
						break;
					case CHUNK_MODEL:
						chunk = new ModelChunk(name.ToString(), startOffset, size, unknown);
						break;
					case CHUNK_MODEL_BINDING:
						chunk = new ModelBindingChunk(name.ToString(), startOffset, size, unknown);
						break;
					case CHUNK_SHADER_VERTEX:
						chunk = new ShaderChunk(name.ToString(), startOffset, size, unknown, ShaderType.Vertex);
						break;
					case CHUNK_SHADER_FRAGMENT:
						chunk = new ShaderChunk(name.ToString(), startOffset, size, unknown, ShaderType.Fragment);
						break;
					case CHUNK_TEXTURE:
						chunk = new TextureChunk(name.ToString(), startOffset, size, unknown);
						break;
					default:
						throw new Exception($"Unknown asset type: {type}");
				}
				Chunks.Add(chunk);

				i += 14 + nameLen;
			}

			foreach (AbstractChunk chunk in Chunks)
			{
				if (chunk.Size == 0)
					continue;

				chunk.Init(new byte[chunk.Size]);
				Buffer.BlockCopy(sourceFileBytes, (int)chunk.StartOffset, chunk.Buffer, 0, chunk.Buffer.Length);

				string fileName = Path.Combine(outputPath, $"{chunk.Name}{chunk.FileExtension}");
				if (chunk.TryExtract(out byte[] result))
					File.WriteAllBytes(fileName, result);
			}
		}
	}
}