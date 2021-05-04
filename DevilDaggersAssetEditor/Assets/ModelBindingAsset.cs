using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersCore.Mods;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, bool isProhibited)
			: base(assetName, AssetType.ModelBinding, isProhibited)
		{
		}

		public override UserAsset ToUserAsset()
			=> new(AssetType.ModelBinding, AssetName, EditorPath);
	}
}
