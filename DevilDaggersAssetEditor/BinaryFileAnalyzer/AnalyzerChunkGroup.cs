using DevilDaggersAssetCore.Chunks;

namespace DevilDaggersAssetCore.BinaryFileAnalyzer
{
	public class AnalyzerChunkGroup
	{
		public byte r;
		public byte g;
		public byte b;
		public uint byteCount;
		public AbstractChunk[] chunks;

		public AnalyzerChunkGroup(byte r, byte g, byte b, uint byteCount, AbstractChunk[] chunks)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.byteCount = byteCount;
			this.chunks = chunks;
		}
	}
}