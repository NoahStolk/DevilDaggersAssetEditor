using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows;

public partial class ConvertImageToGlslCodeWindow : Window
{
	public ConvertImageToGlslCodeWindow()
	{
		InitializeComponent();
	}

	private void BrowseButton_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openDialog = new() { Filter = "PNG files (*.png)|*.png", Title = "Select image to convert to GLSL code" };

		bool? openResult = openDialog.ShowDialog();
		if (!openResult.HasValue || !openResult.Value)
			return;

		ImageResult ir = GetImageResult(openDialog.FileName);

		int arrayLength = ir.Colors.Length / 3;
		GlslCode.Text = $"const int image_width = {ir.Width};\nconst int image_height = {ir.Height};\nconst int[{arrayLength}] image_colors = int[{arrayLength}]({ToColorArrayString(ir.Colors)});";

		static ImageResult GetImageResult(string path)
		{
			using Bitmap image = new(Image.FromFile(path));
			using MemoryStream ms = new();
			using BinaryWriter bw = new(ms);
			for (int x = 0; x < image.Width; x++)
			{
				for (int y = 0; y < image.Height; y++)
				{
					Color pixel = image.GetPixel(x, y);

					bw.Write(pixel.R);
					bw.Write(pixel.G);
					bw.Write(pixel.B);
				}
			}

			return new(image.Width, image.Height, ms.ToArray());
		}

		static string ToColorArrayString(byte[] colorBytes)
		{
			string[] colors = new string[colorBytes.Length / 3];
			for (int i = 0; i < colorBytes.Length; i += 3)
				colors[i / 3] = $"0x{colorBytes[i]:x2}{colorBytes[i + 1]:x2}{colorBytes[i + 2]:x2}";

			return string.Join(',', colors);
		}
	}

	private void CopyButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(GlslCode.Text))
			return;

		Clipboard.SetText(GlslCode.Text);
	}

	private sealed record ImageResult(int Width, int Height, byte[] Colors);
}