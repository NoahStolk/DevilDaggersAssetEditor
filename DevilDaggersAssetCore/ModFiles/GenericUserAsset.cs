namespace DevilDaggersAssetCore.ModFiles
{
	public class GenericUserAsset
	{
		public string AssetName { get; }
		public string EditorPath { get; set; }

		public GenericUserAsset(string assetName, string editorPath)
		{
			AssetName = assetName;
			EditorPath = editorPath;
		}
	}
}