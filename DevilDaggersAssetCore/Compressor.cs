using DevilDaggersAssetCore.Chunks;
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

			// Create file.
			using (FileStream fs = File.Create(outputPath))
			{
				int fileStreamPosition = 0;

				// Write magics.
				fs.Write(BitConverter.GetBytes(Utils.Magic1), fileStreamPosition, 4);
				fileStreamPosition += 4;
				fs.Write(BitConverter.GetBytes(Utils.Magic2), fileStreamPosition, 4);
				fileStreamPosition += 4;

				// Write TOC.
				byte[] tocBuffer = new byte[] { };

				foreach (KeyValuePair<ChunkInfo, List<string>> assetCollection in assetCollections)
				{
					int tocBufferPosition = 0;
					foreach (string assetPath in assetCollection.Value)
					{
						// Write asset type.
						Buffer.BlockCopy(BitConverter.GetBytes(assetCollection.Key.BinaryTypes[0]/*TODO: Shader types*/), 0, tocBuffer, tocBufferPosition, sizeof(byte));
						tocBufferPosition += sizeof(byte);

						// Write name.
						string name = Path.GetFileNameWithoutExtension(assetPath);
						Buffer.BlockCopy(Encoding.Default.GetBytes(name), 0, tocBuffer, tocBufferPosition, name.Length);
						tocBufferPosition += name.Length + 2;

						// Write start offset.
						// TODO: Get start offset in asset data. (Probably need to create that buffer first then...)
						tocBufferPosition += sizeof(int);

						// Write size.
						Buffer.BlockCopy(BitConverter.GetBytes((uint)new FileInfo(assetPath).Length), 0, tocBuffer, tocBufferPosition, sizeof(uint));
						tocBufferPosition += sizeof(int);

						// TODO: Figure out unknown value and write...
						// No reason to write anything for now.
						tocBufferPosition += sizeof(int);
					}
				}

				fs.Write(tocBuffer, 8, tocBuffer.Length);
				fileStreamPosition += tocBuffer.Length;

				// Write asset data.
				foreach (KeyValuePair<ChunkInfo, List<string>> assetCollection in assetCollections)
				{
					foreach (string asset in assetCollection.Value)
					{
						byte[] bytes = File.ReadAllBytes(asset);

						fs.Write(bytes, fileStreamPosition, bytes.Length);
						fileStreamPosition += bytes.Length;
					}
				}
			}
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