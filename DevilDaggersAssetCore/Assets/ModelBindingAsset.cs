using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ModelBindingAsset : AbstractAsset
	{
		public override byte ColorR => 0;
		public override byte ColorG => 255;
		public override byte ColorB => 255;

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