using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Chunks;
using System;

namespace DevilDaggersAssetExtractorConsole
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Extractor extractor = new Extractor();
			extractor.Extract(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd", "DD");
			foreach (AbstractChunk chunk in extractor.Chunks)
				Console.WriteLine(chunk.Name);
		}
	}
}