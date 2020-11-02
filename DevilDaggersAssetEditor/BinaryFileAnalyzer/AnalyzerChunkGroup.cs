using DevilDaggersAssetEditor.Chunks;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.BinaryFileAnalyzer
{
	public class AnalyzerChunkGroup
	{
		public AnalyzerChunkGroup(byte r, byte g, byte b, uint byteCount, List<IChunk> chunks)
		{
			R = r;
			G = g;
			B = b;
			ByteCount = byteCount;
			Chunks = chunks;
		}

		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public uint ByteCount { get; }
		public List<IChunk> Chunks { get; }
	}
}