using DevilDaggersAssetEditor.Chunks;

namespace DevilDaggersAssetEditor.ModFiles
{
	public class ModelBindingUserAsset : AbstractUserAsset
	{
		public override string ChunkTypeName => nameof(ModelBindingChunk);

		public ModelBindingUserAsset(string assetName, string editorPath)
			: base(assetName, editorPath)
		{
		}
	}
}