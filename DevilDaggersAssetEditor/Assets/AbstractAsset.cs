using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public abstract class AbstractAsset
	{
		protected AbstractAsset(string assetName, AssetType assetType, string description, List<string> tags, bool isProhibited)
		{
			AssetName = assetName;
			AssetType = assetType;
			Description = description;
			Tags = tags;
			IsProhibited = isProhibited;
		}

		public string AssetName { get; }
		public AssetType AssetType { get; }
		public string Description { get; }
		public List<string> Tags { get; }
		public bool IsProhibited { get; set; }

		public string EditorPath { get; set; } = GuiUtils.FileNotFound;

		public abstract UserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(UserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}
