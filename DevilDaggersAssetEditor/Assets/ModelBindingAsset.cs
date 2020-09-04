using DevilDaggersAssetEditor.ModFiles;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public ModelBindingAsset(string assetName, string description, string[] tags, string chunkTypeName)
			: base(assetName, description, tags, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ModelBindingUserAsset(AssetName, EditorPath);
	}
}