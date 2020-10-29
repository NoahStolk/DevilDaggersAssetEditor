using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Chunks
{
	public class AudioChunk : ResourceChunk, IChunk
	{
		public AudioChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size)
		{
		}

		public override AssetType AssetType => AssetType.Audio;
	}
}