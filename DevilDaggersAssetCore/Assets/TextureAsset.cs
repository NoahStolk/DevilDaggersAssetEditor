using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class TextureAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 128;
		public override byte ColorB => 0;

		public TextureAsset(string assetName, string description, string chunkTypeName)
			: base(assetName, description, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset()
		{
			return new TextureUserAsset(AssetName, EditorPath);
		}
	}
}