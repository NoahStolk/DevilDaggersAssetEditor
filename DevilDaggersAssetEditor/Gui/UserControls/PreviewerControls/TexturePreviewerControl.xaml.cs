using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SdImage = System.Drawing.Image;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
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
			DefaultMipmaps.Text = TextureAsset.GetMipmapCount(asset.DefaultDimensions.X, asset.DefaultDimensions.Y).ToString();

			bool isPathValid = asset.EditorPath.GetPathValidity() == PathValidity.Valid;

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : Utils.GetPathValidityMessage(asset.EditorPath);

			if (isPathValid)
			{
				using SdImage image = SdImage.FromFile(asset.EditorPath);
				FileDimensions.Text = $"{image.Width}x{image.Height}";
				FileMipmaps.Text = TextureAsset.GetMipmapCount(image.Width, image.Height).ToString();

				using FileStream stream = new FileStream(asset.EditorPath, FileMode.Open, FileAccess.Read);
				BitmapImage src = new BitmapImage();
				src.BeginInit();
				src.StreamSource = stream;
				src.EndInit();
				PreviewImage.Source = src;
			}
			else
			{
				// TODO: Clear image
				FileDimensions.Text = "N/A";
				FileMipmaps.Text = "N/A";
			}
		}
	}
}