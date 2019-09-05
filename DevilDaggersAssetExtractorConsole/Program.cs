using DevilDaggersAssetCore;

namespace DevilDaggersAssetExtractorConsole
{
	public static class Program
	{
		public static void Main()
		{
			Extractor.Extract(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\audio", "Extracted");
			Extractor.Extract(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd", "Extracted");
			//Compressor.Compress(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetExtractorConsole\bin\Debug\Extracted\Audio", @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\audio");
		}
	}
}