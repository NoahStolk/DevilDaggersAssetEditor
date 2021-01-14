using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ParticleChunk : IChunk
	{
		public ParticleChunk(string name, uint startOffset, uint size, byte[] buffer)
		{
			Name = name;
			StartOffset = startOffset;
			Size = size;
			Buffer = buffer;
		}

		public string Name { get; }
		public uint StartOffset { get; }
		public uint Size { get; }
		public byte[] Buffer { get; }

		public AssetType AssetType => AssetType.Particle;
	}
}
