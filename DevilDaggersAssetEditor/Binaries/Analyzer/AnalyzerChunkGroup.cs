using DevilDaggersAssetEditor.Binaries.Chunks;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Binaries.Analyzer
{
	public class AnalyzerChunkGroup
	{
		public AnalyzerChunkGroup(byte r, byte g, byte b, int byteCount, List<Chunk> chunks)
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
		public int ByteCount { get; }
		public List<Chunk> Chunks { get; }
	}
}
