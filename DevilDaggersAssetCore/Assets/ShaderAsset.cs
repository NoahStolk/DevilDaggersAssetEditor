using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ShaderAsset : AbstractAsset
	{
		public override byte ColorR => 0;
		public override byte ColorG => 255;
		public override byte ColorB => 0;

		public ShaderAsset(string assetName, string description, string chunkTypeName)
			: base(assetName, description, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset()
		{
			return new ShaderUserAsset(AssetName, EditorPath);
		}
	}
}