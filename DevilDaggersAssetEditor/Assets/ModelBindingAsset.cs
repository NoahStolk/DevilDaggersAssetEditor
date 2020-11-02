using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, string description, List<string> tags)
			: base(assetName, AssetType.ModelBinding, description, tags)
		{
		}

		public override UserAsset ToUserAsset()
			=> new UserAsset(AssetType.ModelBinding, AssetName, EditorPath);
	}
}