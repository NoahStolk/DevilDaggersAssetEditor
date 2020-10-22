using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using System.Collections.Generic;

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
	}
}