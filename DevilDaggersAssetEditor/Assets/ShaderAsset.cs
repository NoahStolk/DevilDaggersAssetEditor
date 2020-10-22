using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ShaderAsset : AbstractAsset
	{
		public ShaderAsset(string assetName, string description, List<string> tags)
			: base(assetName, AssetType.Shader, description, tags)
		{
		}

		public string FragmentEditorPath { get; set; } = GuiUtils.FileNotFound;

		public override UserAsset ToUserAsset()
			=> new ShaderUserAsset(AssetName, EditorPath, FragmentEditorPath);
	}
}