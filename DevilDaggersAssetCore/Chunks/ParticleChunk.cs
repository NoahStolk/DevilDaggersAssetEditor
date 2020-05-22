using DevilDaggersAssetCore.Headers;

namespace DevilDaggersAssetCore.Chunks
{
	public class ParticleChunk : AbstractHeaderedChunk<ParticleHeader>
	{
		public ParticleChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size, 0) // TODO: Fix hierarchy issue: Unknown value is only present in resource chunks...
		{
		}
	}
}