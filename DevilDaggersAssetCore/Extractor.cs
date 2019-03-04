using DevilDaggersAssetCore.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevilDaggersAssetCore
{
	public static class Extractor
	{
		public static void Extract(string inputPath, string outputPath)
		{
			CreateFolders(outputPath);

			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			uint magic2FromFile = BitConverter.ToUInt32(sourceFileBytes, 4);
			uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);

			ulong magic1 = Utils.MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
			ulong magic2 = Utils.MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);
			if (magic1FromFile != magic1 && magic2FromFile != magic2)
				throw new Exception($"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {magic1FromFile} should be {magic1}\nHeader value 2: {magic2FromFile} should be {magic2}");

			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)tocSize);

			List<AbstractChunk> chunks = CreateChunks(tocBuffer).ToList();
			CreateFiles(outputPath, sourceFileBytes, chunks);
		}

		private static void CreateFolders(string outputPath)
		{
			// This is ridiculously ugly but I can't think of a better automatic way at the moment
			List<Type> chunkTypes = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(AbstractChunk)) && !t.IsAbstract)
				.ToList();
			foreach (Type type in chunkTypes)
				Directory.CreateDirectory(Path.Combine(outputPath, type.GetProperties().Where(p => p.Name == "FolderName").FirstOrDefault().GetValue(Activator.CreateInstance(type, "", (uint)0, (uint)0, (uint)0)) as string));
		}

		private static IEnumerable<AbstractChunk> CreateChunks(byte[] tocBuffer)
		{
			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				ushort type = tocBuffer[i];
				StringBuilder nameS = new StringBuilder();
				int j = 0;
				for (; ; )
				{
					j++;
					char c = (char)tocBuffer[i + j + 1];
					if (c == '\0')
						break;
					nameS.Append(c);
				}
				string name = nameS.ToString();
				int nameLen = name.Length;
				i += nameLen;
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				uint unknown = BitConverter.ToUInt32(tocBuffer, i + 10);
				i += 14;

				AbstractChunk chunk;
				switch (type)
				{
					case Utils.ChunkAudio:
						chunk = new AudioChunk(name.ToString(), startOffset, size, unknown);
						break;
					case Utils.ChunkModel:
						chunk = new ModelChunk(name.ToString(), startOffset, size, unknown);
						break;
					case Utils.ChunkModelBinding:
						chunk = new ModelBindingChunk(name.ToString(), startOffset, size, unknown);
						break;
					case Utils.ChunkShaderVertex:
					case Utils.ChunkShaderFragment:
						chunk = new ShaderChunk(name.ToString(), startOffset, size, unknown);
						break;
					case Utils.ChunkTexture:
						chunk = new TextureChunk(name.ToString(), startOffset, size, unknown);
						break;
					default:
						throw new Exception($"Unknown asset type: {type}");
				}
				yield return chunk;
			}
		}

		private static void CreateFiles(string outputPath, byte[] sourceFileBytes, List<AbstractChunk> chunks)
		{
			foreach (AbstractChunk chunk in chunks)
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