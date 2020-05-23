using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ParticleAsset : AbstractAsset
	{
		public ParticleAsset(string assetName, string description, string[] tags, string chunkTypeName)
			: base(assetName, description, tags, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ParticleUserAsset(AssetName, EditorPath);
	}
}