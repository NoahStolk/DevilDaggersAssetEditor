using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DevilDaggersAssetEditor.GUI.UserControls.PreviewerControls
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
			DefaultDimensions.Text = $"{asset.DefaultDimensions.X}, {asset.DefaultDimensions.Y}";
			DefaultMipmaps.Text = TextureAsset.GetMipmapCount(asset.DefaultDimensions.X, asset.DefaultDimensions.Y).ToString();

			bool isPathValid = asset.EditorPath.IsPathValid();

			FilePath.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : asset.EditorPath;

			if (isPathValid)
			{
				System.Drawing.Image image = System.Drawing.Image.FromFile(asset.EditorPath);
				FileDimensions.Text = $"{image.Width}, {image.Height}";
				FileMipmaps.Text = TextureAsset.GetMipmapCount(image.Width, image.Height).ToString();

				FileStream stream = new FileStream(asset.EditorPath, FileMode.Open, FileAccess.Read);
				BitmapImage src = new BitmapImage();
				src.BeginInit();
				src.StreamSource = stream;
				src.EndInit();
				PreviewImage.Source = src;
			}
		}
	}
}