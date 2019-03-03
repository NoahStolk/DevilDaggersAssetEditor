using DevilDaggersAssetCore.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevilDaggersAssetCore
{
	public class Extractor
	{
		public const ushort ChunkModel = 0x01;
		public const ushort ChunkTexture = 0x02;
		public const ushort ChunkShaderVertex = 0x10;
		public const ushort ChunkShaderFragment = 0x11;
		public const ushort ChunkAudio = 0x20;
		public const ushort ChunkModelBinding = 0x80;

		public static ulong Magic1;
		public static ulong Magic2;

		static Extractor()
		{
			Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
			Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);
		}

		public static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
		{
			return a | b << 8 | c << 16 | d << 24;
		}

		public List<AbstractChunk> Chunks { get; set; } = new List<AbstractChunk>();

		public void Extract(string inputPath, string outputPath)
		{
			Chunks.Clear();

			List<Type> chunkTypes = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(AbstractChunk)) && !t.IsAbstract)
				.ToList();
			foreach (Type type in chunkTypes)
				Directory.CreateDirectory(Path.Combine(outputPath, type.GetProperties().Where(p => p.Name == "FolderName").FirstOrDefault().GetValue(Activator.CreateInstance(type, "", (uint)0, (uint)0, (uint)0)) as string));

			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			FileHeader archiveHeader = new FileHeader(
				magicNumber1: BitConverter.ToUInt32(sourceFileBytes, 0),
				magicNumber2: BitConverter.ToUInt32(sourceFileBytes, 4),
				tocSize: BitConverter.ToUInt32(sourceFileBytes, 8)
			);

			if (archiveHeader.MagicNumber1 != Magic1 && archiveHeader.MagicNumber2 != Magic2)
				throw new Exception($"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {archiveHeader.MagicNumber1} should be {Magic1}\nHeader value 2: {archiveHeader.MagicNumber2} should be {Magic2}");

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
					case ChunkAudio:
						chunk = new AudioChunk(name.ToString(), startOffset, size, unknown);
						break;
					case ChunkModel:
						chunk = new ModelChunk(name.ToString(), startOffset, size, unknown);
						break;
					case ChunkModelBinding:
						chunk = new ModelBindingChunk(name.ToString(), startOffset, size, unknown);
						break;
					case ChunkShaderVertex:
					case ChunkShaderFragment:
						chunk = new ShaderChunk(name.ToString(), startOffset, size, unknown);
						break;
					case ChunkTexture:
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

				byte[] buf = new byte[chunk.Size];
				Buffer.BlockCopy(sourceFileBytes, (int)chunk.StartOffset, buf, 0, (int)chunk.Size);

				chunk.Init(buf);

				string folder = Path.Combine(outputPath, chunk.FolderName);
				foreach (FileResult fileResult in chunk.Extract())
				{
					string fileName = $"{fileResult.Name}{chunk.FileExtension}";

					// Ugly but whatever
					if (fileName == "loudness.wav")
						fileName = "loudness.txt";

					File.WriteAllBytes(Path.Combine(folder, fileName), fileResult.Buffer);
				}
			}
		}
	}
}