using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SdImage = System.Drawing.Image;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
{
	public partial class TexturePreviewerControl : UserControl
	{
		public TexturePreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(AbstractAsset a)
		{
			TextureAsset asset = a as TextureAsset;

			TextureName.Content = asset.AssetName;
			DefaultDimensions.Content = $"{asset.DefaultDimensions.X}x{asset.DefaultDimensions.Y}";
			DefaultMipmaps.Content = TextureAsset.GetMipmapCount(asset.DefaultDimensions.X, asset.DefaultDimensions.Y).ToString(CultureInfo.InvariantCulture);

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Content = isPathValid ? Path.GetFileName(asset.EditorPath) : GuiUtils.FileNotFound;

			if (isPathValid)
			{
				using (SdImage image = SdImage.FromFile(asset.EditorPath))
				{
					FileDimensions.Content = $"{image.Width}x{image.Height}";
					FileMipmaps.Content = TextureAsset.GetMipmapCount(image.Width, image.Height).ToString(CultureInfo.InvariantCulture);
				}

				using FileStream imageFileStream = new FileStream(asset.EditorPath, FileMode.Open, FileAccess.Read);
				MemoryStream imageCopyStream = new MemoryStream();
				imageFileStream.CopyTo(imageCopyStream);
				BitmapImage src = new BitmapImage();
				src.BeginInit();
				src.StreamSource = imageCopyStream;
				src.EndInit();
				PreviewImage.Source = src;
			}
			else
			{
				PreviewImage.Source = null;
				FileDimensions.Content = "N/A";
				FileMipmaps.Content = "N/A";
			}
		}
	}
}