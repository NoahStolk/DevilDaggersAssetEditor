using DevilDaggersAssetCore;

namespace DevilDaggersAssetExtractorConsole
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Extractor.Extract(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\audio", "Extracted");
			Extractor.Extract(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd", "Extracted");
		}
	}
}