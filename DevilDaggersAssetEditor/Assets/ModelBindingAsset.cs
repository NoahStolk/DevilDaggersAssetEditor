using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersCore.Mods;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, string description, bool isProhibited, List<string> tags)
			: base(assetName, AssetType.ModelBinding, description, isProhibited, tags)
		{
		}

		public override UserAsset ToUserAsset()
			=> new(AssetType.ModelBinding, AssetName, EditorPath);
	}
}
