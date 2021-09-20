using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersCore.Mods;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public abstract class AbstractAsset
	{
		protected AbstractAsset(string assetName, AssetType assetType, bool isProhibited)
		{
			AssetName = assetName;
			AssetType = assetType;
			IsProhibited = isProhibited;
		}

		public string AssetName { get; }
		public AssetType AssetType { get; }
		public bool IsProhibited { get; set; }

		public string? Description { get; set; }
		public List<string> Tags { get; set; } = new();

		public string EditorPath { get; set; } = GuiUtils.FileNotFound;

		public abstract UserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(UserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}
