using DevilDaggersCore.Mods;

namespace DevilDaggersAssetEditor.ModFiles
{
	public class UserAsset
	{
		public UserAsset(AssetType assetType, string assetName, string editorPath)
		{
			AssetType = assetType;
			AssetName = assetName;
			EditorPath = editorPath;
		}

		public AssetType AssetType { get; }
		public string AssetName { get; }
		public string EditorPath { get; set; }
	}
}
