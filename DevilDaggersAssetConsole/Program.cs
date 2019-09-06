using DevilDaggersAssetCore;
using System;
using System.IO;

namespace DevilDaggersAssetConsole
{
	public static class Program
	{
		private static readonly string ddResPath = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\";

		public static void Main()
		{
			Console.WriteLine("Devil Daggers Asset Console - Test Program\n");
			Console.WriteLine("Type 'ea' to extract audio");
			Console.WriteLine("Type 'ed' to extract dd");
			Console.WriteLine("Type 'ca' to compress audio");
			Console.WriteLine("Type 'cd' to compress dd");
			string input = Console.ReadLine();

			switch (input)
			{
				case "ea": Extractor.Extract(Path.Combine(ddResPath, "audio"), "Assets"); break;
				case "ed": Extractor.Extract(Path.Combine(ddResPath, "dd"), "Assets"); break;
				case "ca": Compressor.Compress(@"Extracted\", Path.Combine(ddResPath, "audio"), BinaryFileName.Audio); break;
				case "cd": Compressor.Compress(@"Extracted\", Path.Combine(ddResPath, "dd"), BinaryFileName.DD); break;
			}
		}
	}
}