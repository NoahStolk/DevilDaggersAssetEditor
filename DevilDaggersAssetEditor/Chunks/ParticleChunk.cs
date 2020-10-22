using DevilDaggersAssetEditor.Headers;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ParticleChunk : AbstractHeaderedChunk<ParticleHeader>
	{
		public ParticleChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size)
		{
		}
	}
}