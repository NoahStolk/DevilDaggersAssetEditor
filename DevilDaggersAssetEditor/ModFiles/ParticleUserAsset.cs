using DevilDaggersAssetEditor.Chunks;

namespace DevilDaggersAssetEditor.ModFiles
{
	public class ParticleUserAsset : AbstractUserAsset
	{
		public override string ChunkTypeName => nameof(ParticleChunk);

		public ParticleUserAsset(string assetName, string editorPath)
			: base(assetName, editorPath)
		{
		}
	}
}