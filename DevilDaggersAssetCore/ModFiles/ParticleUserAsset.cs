namespace DevilDaggersAssetCore.ModFiles
{
	public class ParticleUserAsset : AbstractUserAsset
	{
		// TODO: Create a distiction between particles and other asset types in the hierarchy.
		public override string ChunkTypeName => "";

		public ParticleUserAsset(string assetName, string editorPath)
			: base(assetName, editorPath)
		{
		}
	}
}