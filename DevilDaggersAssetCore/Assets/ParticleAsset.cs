using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ParticleAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 255;
		public override byte ColorB => 0;

		public ParticleAsset(string assetName, string description, string chunkTypeName)
			: base(assetName, description, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset()
		{
			return new ParticleUserAsset(AssetName, EditorPath);
		}
	}
}