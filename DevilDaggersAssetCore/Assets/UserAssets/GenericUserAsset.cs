namespace DevilDaggersAssetCore.Assets.UserAssets
{
	public class GenericUserAsset
	{
		public string AssetName { get; }
		public string EditorPath { get; }

		public GenericUserAsset(string assetName, string editorPath)
		{
			AssetName = assetName;
			EditorPath = editorPath;
		}
	}
}