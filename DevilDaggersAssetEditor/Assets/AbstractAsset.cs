using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public abstract class AbstractAsset
	{
		protected AbstractAsset(string assetName, AssetType assetType, string description, List<string> tags)
		{
			AssetName = assetName;
			AssetType = assetType;
			Description = description;
			Tags = tags;
		}

		public string AssetName { get; }
		public AssetType AssetType { get; }
		public string Description { get; }
		public List<string> Tags { get; }

		public string EditorPath { get; set; } = GuiUtils.FileNotFound;

		public abstract UserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(UserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}