using DevilDaggersCore.Mods;

namespace DevilDaggersAssetEditor.ModFiles;

public class ShaderUserAsset : UserAsset
{
	public ShaderUserAsset(string assetName, string vertexEditorPath, string fragmentEditorPath)
		: base(AssetType.Shader, assetName, vertexEditorPath)
	{
		EditorPathFragmentShader = fragmentEditorPath;
	}

	public string EditorPathFragmentShader { get; set; }
}