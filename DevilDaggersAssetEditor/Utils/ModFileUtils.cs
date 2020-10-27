using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Utils
{
	public static class ModFileUtils
	{
		public static List<UserAsset> GetAssetsFromModFilePath(string path)
		{
			List<UserAsset>? assets = JsonFileUtils.TryDeserializeFromFile<List<UserAsset>>(path, true);
			if (assets == null)
				return new List<UserAsset>();

			UserHandler.Instance.Cache.OpenedModFilePath = path;

			return assets;
		}

		/// <summary>
		/// Creates a mod file based on the assets found in the directory specified by <paramref name="path"/> and places it inside that directory.
		/// </summary>
		public static void CreateModFileFromPath(string path)
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
				AssetType? assetType = Path.GetExtension(path).GetAssetTypeFromFileExtension();
				if (!assetType.HasValue)
					continue;

				if (assetType == AssetType.Shader)
				{
					if (!assets.Any(a => a.AssetType == AssetType.Shader && a.AssetName == name))
					{
						string normalizedPath = path.TrimEnd("_vertex.glsl").TrimEnd("_fragment.glsl");

						assets.Add(new ShaderUserAsset(
							name.TrimEnd("_vertex").TrimEnd("_fragment"),
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
					assets.Add(new UserAsset(assetType.Value, name, path));
				}
			}

			return assets;
		}
	}
}