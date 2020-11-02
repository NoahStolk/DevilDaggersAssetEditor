using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ParticleChunk : IChunk
	{
		public ParticleChunk(string name, uint startOffset, uint size)
		{
			Name = name;
			StartOffset = startOffset;
			Size = size;
		}

		public string Name { get; set; }
		public uint StartOffset { get; set; }
		public uint Size { get; set; }

		public byte[] Buffer { get; set; }

		public AssetType AssetType => AssetType.Particle;
	}
}