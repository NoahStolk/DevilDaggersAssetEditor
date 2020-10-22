using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Info;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public abstract class AbstractBinaryFileHandler
	{
		protected AbstractBinaryFileHandler(BinaryFileType binaryFileType)
		{
			BinaryFileType = binaryFileType;
		}

		public BinaryFileType BinaryFileType { get; }

		public abstract void MakeBinary(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription);

		public abstract void ExtractBinary(string inputPath, string outputPath, BinaryFileType binaryFileType, Progress<float> progress, Progress<string> progressDescription);

		public abstract void ValidateFile(byte[] sourceFileBytes);

		protected static void CreateModFile(string path)
		{
			List<UserAsset> assets = GetAssets(path);

			string folderName = new DirectoryInfo(path).Name;
			JsonFileUtils.SerializeToFile(Path.Combine(path, $"{folderName}.ddae"), assets, true);
		}

		private static List<UserAsset> GetAssets(string directory)
		{
			Dictionary<string, float> loudnessValues = new Dictionary<string, float>();
			string? loudnessFilePath = Array.Find(Directory.GetFiles(directory, "*.ini", SearchOption.AllDirectories), p => Path.GetFileNameWithoutExtension(p) == "loudness");
			if (loudnessFilePath != null)
			{
				foreach (string line in File.ReadAllLines(loudnessFilePath))
				{
					if (LoudnessUtils.TryReadLoudnessLine(line, out string? assetName, out float loudness))
						loudnessValues.Add(assetName!, loudness);
				}
			}

			List<UserAsset> assets = new List<UserAsset>();

			foreach (string path in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
			{
				string name = Path.GetFileNameWithoutExtension(path);

				string? extension = Path.GetExtension(path);
				AssetType assetType = ChunkInfo.All.FirstOrDefault(e => e.FileExtension == extension)!.AssetType;

				if (assetType == AssetType.Shader)
				{
					if (!assets.Any(a => a.AssetType == AssetType.Shader && a.AssetName == name))
					{
						string normalizedName = name
							.Replace("_vertex", string.Empty, StringComparison.InvariantCulture)
							.Replace("_fragment", string.Empty, StringComparison.InvariantCulture);

						string normalizedPath = path
							.Replace("_vertex.glsl", string.Empty, StringComparison.InvariantCulture)
							.Replace("_fragment.glsl", string.Empty, StringComparison.InvariantCulture);

						assets.Add(new ShaderUserAsset(
							normalizedName,
							$"{normalizedPath}_vertex.glsl",
							$"{normalizedPath}_fragment.glsl"));
					}
				}
				else if (assetType == AssetType.Audio)
				{
					float loudness = 1;
					if (loudnessValues.ContainsKey(name))
						loudness = loudnessValues[name];
					assets.Add(new AudioUserAsset(name, path, loudness));
				}
				else
				{
					assets.Add(new UserAsset(assetType, name, path));
				}
			}

			return assets;
		}
	}
}