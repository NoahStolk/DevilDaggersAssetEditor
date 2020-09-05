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

		public void Initialize(TextureAsset asset)
		{
			TextureName.Text = asset.AssetName;
			DefaultDimensions.Text = $"{asset.DefaultDimensions.X}x{asset.DefaultDimensions.Y}";
			DefaultMipmaps.Text = TextureAsset.GetMipmapCount(asset.DefaultDimensions.X, asset.DefaultDimensions.Y).ToString(CultureInfo.InvariantCulture);

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : GuiUtils.FileNotFound;

			if (isPathValid)
			{
				using (SdImage image = SdImage.FromFile(asset.EditorPath))
				{
					FileDimensions.Text = $"{image.Width}x{image.Height}";
					FileMipmaps.Text = TextureAsset.GetMipmapCount(image.Width, image.Height).ToString(CultureInfo.InvariantCulture);
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
				FileDimensions.Text = "N/A";
				FileMipmaps.Text = "N/A";
			}
		}
	}
}