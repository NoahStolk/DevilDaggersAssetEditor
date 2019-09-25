using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, string description, string chunkTypeName)
			: base(assetName, description, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset()
		{
			return new ModelBindingUserAsset(AssetName, EditorPath);
		}
	}
}