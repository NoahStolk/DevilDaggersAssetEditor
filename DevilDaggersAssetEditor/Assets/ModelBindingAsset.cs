using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, string description, List<string> tags, bool isProhibited)
			: base(assetName, AssetType.ModelBinding, description, tags, isProhibited)
		{
		}

		public override UserAsset ToUserAsset()
			=> new(AssetType.ModelBinding, AssetName, EditorPath);
	}
}
