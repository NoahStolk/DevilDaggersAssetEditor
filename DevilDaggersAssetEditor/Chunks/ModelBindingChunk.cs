using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ModelBindingChunk : ResourceChunk
	{
		public ModelBindingChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size)
		{
		}

		public override AssetType AssetType => AssetType.ModelBinding;
	}
}