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
			// Read file contents.
			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			// Validate file.
			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			uint magic2FromFile = BitConverter.ToUInt32(sourceFileBytes, 4);
			if (magic1FromFile != BinaryFileUtils.Magic1 && magic2FromFile != BinaryFileUtils.Magic2)
				throw new Exception($"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {magic1FromFile} should be {BinaryFileUtils.Magic1}\nHeader value 2: {magic2FromFile} should be {BinaryFileUtils.Magic2}");

			// Read toc buffer.
			uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);
			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)tocSize);

			// Create chunks based on toc buffer.
			IEnumerable<AbstractChunk> chunks = ReadChunks(tocBuffer);

			// Create folders and files based on chunks.
			CreateFiles(outputPath, sourceFileBytes, chunks);
		}

		private static IEnumerable<AbstractChunk> ReadChunks(byte[] tocBuffer)
		{
			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				ushort type = BitConverter.ToUInt16(tocBuffer, i);
				string name = BinaryFileUtils.ReadNullTerminatedString(tocBuffer, i + 2);
				i += name.Length + 1; // + 1 to include null terminator.
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				uint unknown = BitConverter.ToUInt32(tocBuffer, i + 10);
				i += 14;

				yield return Activator.CreateInstance(BinaryFileUtils.ChunkInfos.Where(c => c.BinaryTypes.Contains(type)).FirstOrDefault().Type, name, startOffset, size, unknown) as AbstractChunk;
			}
		}

		private static void CreateFiles(string outputPath, byte[] sourceFileBytes, IEnumerable<AbstractChunk> chunks)
		{
			foreach (ChunkInfo info in BinaryFileUtils.ChunkInfos)
				Directory.CreateDirectory(Path.Combine(outputPath, info.FolderName));

			foreach (AbstractChunk chunk in chunks)
			{
				if (chunk.Size == 0)
					continue;

				byte[] buf = new byte[chunk.Size];
				Buffer.BlockCopy(sourceFileBytes, (int)chunk.StartOffset, buf, 0, (int)chunk.Size);

				chunk.Init(buf);

				ChunkInfo info = BinaryFileUtils.ChunkInfos.Where(c => c.Type == chunk.GetType()).FirstOrDefault();
				foreach (FileResult fileResult in chunk.Extract())
				{
					string fileName = $"{fileResult.Name}{info.FileExtension}";

					File.WriteAllBytes(Path.Combine(outputPath, info.FolderName, fileName), fileResult.Buffer);
				}
			}
		}
	}
}