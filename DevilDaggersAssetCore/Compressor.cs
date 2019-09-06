using DevilDaggersAssetCore.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetCore
{
	public static class Compressor
	{
		/// <summary>
		/// Compresses multiple asset files into one binary file that can be read by Devil Daggers.
		/// </summary>
		/// <param name="inputPath">The path containing the asset files.</param>
		/// <param name="outputPath">The path where the compressed binary file will be placed.</param>
		/// <param name="binaryFileName">The binary file name (audio or dd) to know which asset files to compress.</param>
		public static void Compress(string inputPath, string outputPath, BinaryFileName binaryFileName)
		{
			Dictionary<ChunkInfo, List<AbstractChunk>> chunkCollections = GetAssets(inputPath, binaryFileName);

			// Create TOC stream.
			byte[] tocBuffer;
			Dictionary<AbstractChunk, long> startOffsetBytePositions = new Dictionary<AbstractChunk, long>();
			using (MemoryStream tocStream = new MemoryStream())
			{
				foreach (KeyValuePair<ChunkInfo, List<AbstractChunk>> chunkCollection in chunkCollections)
				{
					ushort type = chunkCollection.Key.BinaryTypes[0]; // TODO: Shaders have multiple types.

					foreach (AbstractChunk chunk in chunkCollection.Value)
					{
						// Write asset type.
						tocStream.Write(BitConverter.GetBytes(type), 0, sizeof(byte));
						tocStream.Position++;

						// Write name.
						tocStream.Write(Encoding.Default.GetBytes(chunk.Name), 0, chunk.Name.Length);
						tocStream.Position++;

						// Write start offsets when TOC buffer size is defined.
						startOffsetBytePositions[chunk] = tocStream.Position;
						tocStream.Position += sizeof(uint);

						// Write size.
						tocStream.Write(BitConverter.GetBytes(chunk.Size), 0, sizeof(uint));

						// TODO: Figure out unknown value and write...
						// No reason to write anything for now.
						tocStream.Position += sizeof(uint);
					}
				}
				tocStream.Write(BitConverter.GetBytes(0), 0, 2); // Empty value between TOC and assets.
				tocBuffer = tocStream.ToArray();
			}

			// Create asset stream.
			byte[] assetBuffer;
			using (MemoryStream assetStream = new MemoryStream())
			{
				foreach (KeyValuePair<ChunkInfo, List<AbstractChunk>> assetCollection in chunkCollections)
				{
					foreach (AbstractChunk chunk in assetCollection.Value)
					{
						uint startOffset = (uint)(BinaryFileUtils.HeaderSize + tocBuffer.Length + assetStream.Position);
						chunk.StartOffset = startOffset;

						// Write start offset to TOC stream.
						Buffer.BlockCopy(BitConverter.GetBytes(startOffset), 0, tocBuffer, (int)startOffsetBytePositions[chunk], sizeof(uint));

						// Write asset data to asset stream.
						byte[] bytes = chunk.Compress();
						assetStream.Write(bytes, 0, bytes.Length);
					}
				}
				assetBuffer = assetStream.ToArray();
			}

			// Create file.
			using FileStream fs = File.Create(outputPath);

			// Write file header.
			fs.Write(BitConverter.GetBytes((uint)BinaryFileUtils.Magic1), 0, sizeof(uint));
			fs.Write(BitConverter.GetBytes((uint)BinaryFileUtils.Magic2), 0, sizeof(uint));
			fs.Write(BitConverter.GetBytes((uint)tocBuffer.Length), 0, sizeof(uint));

			// Write TOC buffer.
			fs.Write(tocBuffer, 0, tocBuffer.Length);

			// Write asset buffer.
			fs.Write(assetBuffer, 0, assetBuffer.Length);
		}

		private static Dictionary<ChunkInfo, List<AbstractChunk>> GetAssets(string inputPath, BinaryFileName binaryFileName)
		{
			Dictionary<ChunkInfo, List<AbstractChunk>> assetCollections = new Dictionary<ChunkInfo, List<AbstractChunk>>();
			foreach (ChunkInfo chunkInfo in BinaryFileUtils.ChunkInfos.Where(c => c.BinaryFileName == binaryFileName))
			{
				List<AbstractChunk> assetPaths = new List<AbstractChunk>();
				foreach (string assetPath in Directory.GetFiles(inputPath, $"*{chunkInfo.FileExtension}", SearchOption.AllDirectories))
				{
					string name = Path.GetFileNameWithoutExtension(assetPath);

					string filePath = Path.Combine(inputPath, chunkInfo.FolderName, $"{name}{chunkInfo.FileExtension}");
					FileInfo fileInfo = new FileInfo(filePath);

					uint startOffset = 0; // We don't know the start offset yet, set it when TOC buffer size is defined.
					uint size = (uint)fileInfo.Length;
					uint unknown = 0; // We don't know what the unknown value is supposed to represent, so leave it empty.

					AbstractChunk chunk = (AbstractChunk)Activator.CreateInstance(chunkInfo.Type, name, startOffset, size, unknown);
					chunk.Init(File.ReadAllBytes(filePath));
					assetPaths.Add(chunk);
				}

				assetCollections[chunkInfo] = assetPaths;
			}

			return assetCollections;
		}
	}
}