using DevilDaggersAssetEditor.Binaries.Chunks;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersAssetEditor.Binaries.Analyzer
{
	public class AnalyzerFileResult
	{
		public AnalyzerFileResult(string fileName, int fileByteCount, int headerByteCount, List<Chunk> chunks)
		{
			FileName = fileName;
			FileByteCount = fileByteCount;
			HeaderByteCount = headerByteCount;
			Chunks = chunks.Where(c => c.Size != 0).ToList(); // Filter empty chunks (garbage in TOC buffer).
		}

		public string FileName { get; }
		public int FileByteCount { get; }
		public int HeaderByteCount { get; }
		public List<Chunk> Chunks { get; }
	}
}
