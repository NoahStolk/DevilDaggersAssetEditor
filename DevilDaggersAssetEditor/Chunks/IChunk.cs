using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Chunks
{
	public interface IChunk
	{
		public string Name { get; }
		public uint Size { get; }
		public AssetType AssetType { get; }
	}
}