using System;
using System.Collections.Generic;
using System.IO;
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
		public static void Compress(string inputPath, string outputPath)
		{
			Dictionary<ChunkInfo, List<string>> assetCollections = GetAssets(inputPath);

			// Create TOC stream.
			byte[] tocBuffer;
			Dictionary<string, long> offsetBytePositions = new Dictionary<string, long>();
			using (MemoryStream tocStream = new MemoryStream())
			{
				foreach (KeyValuePair<ChunkInfo, List<string>> assetCollection in assetCollections)
				{
					foreach (string assetPath in assetCollection.Value)
					{
						// Write asset type.
						tocStream.Write(BitConverter.GetBytes(assetCollection.Key.BinaryTypes[0]/*TODO: Shader types*/), 0, sizeof(byte));
						tocStream.Position++;

						// Write name.
						string name = Path.GetFileNameWithoutExtension(assetPath);
						tocStream.Write(Encoding.Default.GetBytes(name), 0, name.Length);
						tocStream.Position++;

						// Write start offsets when TOC buffer size is defined.
						offsetBytePositions[assetPath] = tocStream.Position;
						tocStream.Write(BitConverter.GetBytes(0), 0, sizeof(int));

						// Write size.
						tocStream.Write(BitConverter.GetBytes((uint)new FileInfo(assetPath).Length), 0, sizeof(uint));

						// TODO: Figure out unknown value and write...
						// No reason to write anything for now.
						tocStream.Write(BitConverter.GetBytes(0), 0, sizeof(int));
					}
				}
				tocStream.Write(BitConverter.GetBytes(0), 0, 2);
				tocBuffer = tocStream.ToArray();
			}

			// Create asset stream.
			byte[] assetBuffer;
			using (MemoryStream assetStream = new MemoryStream())
			{
				foreach (KeyValuePair<ChunkInfo, List<string>> assetCollection in assetCollections)
				{
					foreach (string assetPath in assetCollection.Value)
					{
						// Write start offset to TOC stream.
						Buffer.BlockCopy(BitConverter.GetBytes((uint)(12 + tocBuffer.Length + assetStream.Position)), 0, tocBuffer, (int)offsetBytePositions[assetPath], sizeof(uint));

						// Write asset data to asset stream.
						byte[] bytes = File.ReadAllBytes(assetPath);
						assetStream.Write(bytes, 0, bytes.Length);
					}
				}
				assetBuffer = assetStream.ToArray();
			}

			// Create file.
			using FileStream fs = File.Create(outputPath);

			// Write file header.
			fs.Write(BitConverter.GetBytes(Utils.Magic1), 0, sizeof(uint));
			fs.Write(BitConverter.GetBytes(Utils.Magic2), 0, sizeof(uint));
			fs.Write(BitConverter.GetBytes(tocBuffer.Length), 0, sizeof(int));

			// Write TOC buffer.
			fs.Write(tocBuffer, 0, tocBuffer.Length);

			// Write asset buffer.
			fs.Write(assetBuffer, 0, assetBuffer.Length);
		}

		private static Dictionary<ChunkInfo, List<string>> GetAssets(string inputPath)
		{
			Dictionary<ChunkInfo, List<string>> assetCollections = new Dictionary<ChunkInfo, List<string>>();
			foreach (ChunkInfo chunkInfo in Utils.ChunkInfos)
			{
				List<string> assetPaths = new List<string>();
				foreach (string assetPath in Directory.GetFiles(inputPath, $"*{chunkInfo.FileExtension}", SearchOption.AllDirectories))
					assetPaths.Add(assetPath);
				assetCollections[chunkInfo] = assetPaths;
			}

			return assetCollections;
		}
	}
}