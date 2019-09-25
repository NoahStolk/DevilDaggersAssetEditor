using DevilDaggersAssetCore.Chunks;

namespace DevilDaggersAssetCore.ModFiles
{
	public class ModelUserAsset : AbstractUserAsset
	{
		public override string ChunkTypeName => nameof(ModelChunk);

		public ModelUserAsset(string assetName, string editorPath)
			: base(assetName, editorPath)
		{
		}
	}
}