﻿using DevilDaggersAssetCore.Chunks;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersAssetCore.BinaryFileAnalyzer
{
	public class AnalyzerFileResult
	{
		public string fileName;
		public uint fileByteCount;
		public uint headerByteCount;
		public List<AbstractChunk> chunks;

		public AnalyzerFileResult(string fileName, uint fileByteCount, uint headerByteCount, List<AbstractChunk> chunks)
		{
			this.fileName = fileName;
			this.fileByteCount = fileByteCount;
			this.headerByteCount = headerByteCount;
			this.chunks = chunks.Where(c => c.Size != 0).ToList(); // Filter empty chunks (garbage in TOC buffer).
		}
	}
}