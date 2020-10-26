using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ModelBindingChunk : ResourceChunk, IChunk
	{
		public ModelBindingChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size)
		{
		}

		public AssetType AssetType => AssetType.ModelBinding;
	}
}