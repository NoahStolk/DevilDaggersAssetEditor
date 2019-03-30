using DevilDaggersAssetCore.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetCore
{
	public static class Extractor
	{
		/// <summary>
		/// Extracts a compressed binary file into multiple asset files.
		/// </summary>
		/// <param name="inputPath">The path containing the binary file (C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res).</param>
		/// <param name="outputPath">The path where the extracted asset files will be placed.</param>
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
			foreach (ChunkInfo info in Utils.ChunkInfos)
				Directory.CreateDirectory(Path.Combine(outputPath, info.FolderName));
		}

		private static IEnumerable<AbstractChunk> CreateChunks(byte[] tocBuffer)
		{
			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
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
				i += nameLen;
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				uint unknown = BitConverter.ToUInt32(tocBuffer, i + 10);
				i += 14;

				yield return Activator.CreateInstance(Utils.ChunkInfos.Where(c => c.BinaryTypes.Contains(type)).FirstOrDefault().Type, name.ToString(), startOffset, size, unknown) as AbstractChunk;
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

				ChunkInfo info = Utils.ChunkInfos.Where(c => c.Type == chunk.GetType()).FirstOrDefault();
				foreach (FileResult fileResult in chunk.Extract())
				{
					string fileName = $"{fileResult.Name}{info.FileExtension}";

					// Ugly but whatever
					if (fileName == "loudness.wav")
						fileName = "loudness.txt";

					File.WriteAllBytes(Path.Combine(outputPath, info.FolderName, fileName), fileResult.Buffer);
				}
			}
		}
	}
}