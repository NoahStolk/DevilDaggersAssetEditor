using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.User;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.ModFiles
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