using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ModelAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 0;
		public override byte ColorB => 0;

		public ModelAsset(string assetName, string description, string entityName, string chunkTypeName)
			: base(assetName, description, entityName, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ModelUserAsset(AssetName, EditorPath);
	}
}