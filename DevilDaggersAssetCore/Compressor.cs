using DevilDaggersAssetCore.Chunks;
using DevilDaggersAssetCore.Headers;
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
			Dictionary<ChunkInfo, List<AbstractChunk>> chunkCollections = GetChunks(inputPath, binaryFileName);

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

		private static Dictionary<ChunkInfo, List<AbstractChunk>> GetChunks(string inputPath, BinaryFileName binaryFileName)
		{
			Dictionary<ChunkInfo, List<AbstractChunk>> assetCollections = new Dictionary<ChunkInfo, List<AbstractChunk>>();
			foreach (ChunkInfo chunkInfo in BinaryFileUtils.ChunkInfos.Where(c => c.BinaryFileName.HasFlag(binaryFileName)))
			{
				List<AbstractChunk> chunks = new List<AbstractChunk>();

				// Key is chunk name, value is array of file paths belonging to the chunk.
				Dictionary<string, string[]> assets = new Dictionary<string, string[]>();
				string[] chunkNames = Directory.GetFiles(inputPath, $"*{chunkInfo.FileExtension}", SearchOption.AllDirectories).Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();

				// Cut off _fragment and _vertex substrings from shader files to get the shader chunk name.
				if (chunkInfo.FolderName == "Shaders")
					chunkNames = chunkNames.Select(f => f.Replace("_fragment", "").Replace("_vertex", "")).Distinct().ToArray();

				foreach (string chunkName in chunkNames)
				{
					// TODO: Add _fragment and _vertex to Where predicate to get correct shader file paths.
					assets[chunkName] = Directory.GetFiles(inputPath, $"*{chunkInfo.FileExtension}", SearchOption.AllDirectories).Where(f => Path.GetFileNameWithoutExtension(f) == chunkName).ToArray();
				}

				foreach (KeyValuePair<string, string[]> kvp in assets)
				{
					// Create chunk based on file buffer.
					byte[] fileBuffer;
					using (MemoryStream ms = new MemoryStream())
					{
						foreach (string assetFilePath in kvp.Value)
						{
							byte[] fileContents = File.ReadAllBytes(assetFilePath);
							ms.Write(fileContents, 0, fileContents.Length);
						}
						fileBuffer = ms.ToArray();
					}
					AbstractChunk chunk = (AbstractChunk)Activator.CreateInstance(chunkInfo.Type, kvp.Key, 0U/*Don't know start offset yet.*/, (uint)fileBuffer.Length, 0U);

					// Create header based on file buffer and chunk type.
					if (chunkInfo.Type == typeof(AbstractHeaderedChunk<AbstractHeader>))
					{
						AbstractHeaderedChunk<AbstractHeader> headeredChunk = (AbstractHeaderedChunk<AbstractHeader>)Activator.CreateInstance(chunkInfo.Type);

						byte[] headerBuffer = new byte[headeredChunk.Header.ByteCount];
						// TODO: Read fileBuffer and create headerBuffer.

						byte[] chunkBuffer = new byte[headerBuffer.Length + fileBuffer.Length];
						Buffer.BlockCopy(headerBuffer, 0, chunkBuffer, 0, headerBuffer.Length);
						Buffer.BlockCopy(fileBuffer, 0, chunkBuffer, headerBuffer.Length, fileBuffer.Length);

						chunk.Init(chunkBuffer);
					}
					else
					{
						chunk.Init(fileBuffer);
					}

					chunks.Add(chunk);
				}

				assetCollections[chunkInfo] = chunks;
			}

			return assetCollections;
		}
	}
}