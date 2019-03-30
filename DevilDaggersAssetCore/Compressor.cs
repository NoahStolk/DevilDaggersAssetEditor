using DevilDaggersAssetCore.Chunks;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetCore
{
	// TODO
	public static class Compressor
	{
		/// <summary>
		/// Compresses multiple asset files into one binary file that can be read by Devil Daggers.
		/// </summary>
		/// <param name="inputPath">The path containing the asset files.</param>
		/// <param name="outputPath">The path where the compressed binary file will be placed.</param>
		public static void Compress(string inputPath, string outputPath)
		{
			Dictionary<ChunkInfo, string[]> assetFiles = new Dictionary<ChunkInfo, string[]>();
			foreach (ChunkInfo info in Utils.ChunkInfos)
				assetFiles[info] = Directory.GetFiles(inputPath, $"*{info.FileExtension}", SearchOption.AllDirectories);

			List<AbstractChunk> chunks = new List<AbstractChunk>();
			foreach (KeyValuePair<ChunkInfo, string[]> kvp in assetFiles)
			{
				//foreach (string file in kvp.Value)
					//chunks.Add(Activator.CreateInstance(kvp.Key.Type, ));
			}
		}
	}
}