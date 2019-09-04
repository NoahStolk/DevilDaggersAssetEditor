using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PNGEncodeDecodeTest
{
	public static class Program
	{
		private static readonly List<PixelFormat> pfs = new List<PixelFormat>
		{
			PixelFormats.Default,
			PixelFormats.Rgba128Float,
			PixelFormats.Gray32Float,
			PixelFormats.Gray16,
			PixelFormats.Prgba64,
			PixelFormats.Rgba64,
			PixelFormats.Rgb48,
			PixelFormats.Pbgra32,
			PixelFormats.Bgra32,
			PixelFormats.Bgr32,
			PixelFormats.Bgr101010,
			PixelFormats.Rgb24,
			PixelFormats.Bgr24,
			PixelFormats.Rgb128Float,
			PixelFormats.Bgr565,
			PixelFormats.Bgr555,
			PixelFormats.Gray8,
			PixelFormats.Gray4,
			PixelFormats.Gray2,
			PixelFormats.BlackWhite,
			PixelFormats.Indexed8,
			PixelFormats.Indexed4,
			PixelFormats.Indexed2,
			PixelFormats.Indexed1,
			PixelFormats.Prgba128Float,
			PixelFormats.Cmyk32
		};

		private static readonly Dictionary<string, BitmapPalette> bps = new Dictionary<string, BitmapPalette>
		{
			{ "BlackAndWhite", BitmapPalettes.BlackAndWhite },
			{ "Gray256Transparent", BitmapPalettes.Gray256Transparent },
			{ "Gray256", BitmapPalettes.Gray256 },
			{ "Gray16Transparent", BitmapPalettes.Gray16Transparent },
			{ "Gray16", BitmapPalettes.Gray16 },
			{ "Gray4Transparent", BitmapPalettes.Gray4Transparent },
			{ "Gray4", BitmapPalettes.Gray4 },
			{ "Halftone256Transparent", BitmapPalettes.Halftone256Transparent },
			{ "Halftone256", BitmapPalettes.Halftone256 },
			{ "Halftone252Transparent", BitmapPalettes.Halftone252Transparent },
			{ "Halftone252", BitmapPalettes.Halftone252 },
			{ "Halftone216Transparent", BitmapPalettes.Halftone216Transparent },
			{ "Halftone216", BitmapPalettes.Halftone216 },
			{ "Halftone125Transparent", BitmapPalettes.Halftone125Transparent },
			{ "Halftone125", BitmapPalettes.Halftone125 },
			{ "Halftone64Transparent", BitmapPalettes.Halftone64Transparent },
			{ "Halftone64", BitmapPalettes.Halftone64 },
			{ "Halftone27Transparent", BitmapPalettes.Halftone27Transparent },
			{ "Halftone27", BitmapPalettes.Halftone27 },
			{ "Halftone8Transparent", BitmapPalettes.Halftone8Transparent },
			{ "Halftone8", BitmapPalettes.Halftone8 },
			{ "BlackAndWhiteTransparent", BitmapPalettes.BlackAndWhiteTransparent },
			{ "WebPalette", BitmapPalettes.WebPalette },
			{ "WebPaletteTransparent", BitmapPalettes.WebPaletteTransparent }
		};

		public static void Main(string[] args)
		{
			Encode(File.ReadAllBytes(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetExtractorConsole\bin\Debug\netcoreapp2.1\Extracted\Textures\thorn.png"), 256);
		}

		public static void Encode(byte[] buffer, int size)
		{
			foreach (PixelFormat pf in pfs)
				foreach (KeyValuePair<string, BitmapPalette> kvp in bps)
					Write($"{pf}{kvp.Key}", buffer, size, size, pf, kvp.Value);
		}

		private static void Write(string fileName, byte[] buffer, int width, int height, PixelFormat pf, BitmapPalette bp)
		{
			try
			{
				BitmapSource image = BitmapSource.Create(
					pixelWidth: width,
					pixelHeight: height,
					dpiX: 0,
					dpiY: 0,
					pixelFormat: pf,
					palette: bp,
					pixels: buffer,
					stride: width);

				PngBitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(image));
				encoder.Save(new FileStream(Path.Combine("img", $"{fileName}.png"), FileMode.Create));
				Console.WriteLine($"Wrote to {fileName}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}