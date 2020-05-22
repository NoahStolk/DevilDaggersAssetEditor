using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, string description, string entityName, string chunkTypeName)
			: base(assetName, description, entityName, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ModelBindingUserAsset(AssetName, EditorPath);
	}
}