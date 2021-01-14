using DevilDaggersAssetEditor.Chunks;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersAssetEditor.BinaryFileAnalyzer
{
	public class AnalyzerFileResult
	{
		public AnalyzerFileResult(string fileName, uint fileByteCount, uint headerByteCount, List<IChunk> chunks)
		{
			FileName = fileName;
			FileByteCount = fileByteCount;
			HeaderByteCount = headerByteCount;
			Chunks = chunks.Where(c => c.Size != 0).ToList(); // Filter empty chunks (garbage in TOC buffer).
		}

		public string FileName { get; }
		public uint FileByteCount { get; }
		public uint HeaderByteCount { get; }
		public List<IChunk> Chunks { get; }
	}
}
