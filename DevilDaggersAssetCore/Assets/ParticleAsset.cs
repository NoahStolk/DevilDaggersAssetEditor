using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ParticleAsset : AbstractAsset
	{
		public ParticleAsset(string assetName, string description, string entityName, string chunkTypeName)
			: base(assetName, description, entityName, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ParticleUserAsset(AssetName, EditorPath);
	}
}