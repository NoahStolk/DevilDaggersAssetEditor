using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.ModFiles
{
	public class ShaderUserAsset : UserAsset
	{
		public ShaderUserAsset(string assetName, string vertexEditorPath, string fragmentEditorPath)
			: base(AssetType.Shader, assetName, vertexEditorPath)
		{
			FragmentEditorPath = fragmentEditorPath;
		}

		public string FragmentEditorPath { get; set; }
	}
}