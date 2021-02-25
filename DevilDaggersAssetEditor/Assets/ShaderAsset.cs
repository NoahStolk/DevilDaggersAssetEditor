using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ShaderAsset : AbstractAsset
	{
		public ShaderAsset(string assetName, string description, List<string> tags, bool isProhibited)
			: base(assetName, AssetType.Shader, description, tags, isProhibited)
		{
		}

		public string EditorPathFragmentShader { get; set; } = GuiUtils.FileNotFound;

		public override UserAsset ToUserAsset()
			=> new ShaderUserAsset(AssetName, EditorPath, EditorPathFragmentShader);

		public override void ImportValuesFromUserAsset(UserAsset userAsset)
		{
			base.ImportValuesFromUserAsset(userAsset);

			if (userAsset is ShaderUserAsset shaderAsset)
				EditorPathFragmentShader = shaderAsset.EditorPathFragmentShader;
		}
	}
}
