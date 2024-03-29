using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SdImage = System.Drawing.Image;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;

public partial class TexturePreviewerControl : UserControl, IPreviewerControl
{
	public TexturePreviewerControl()
	{
		InitializeComponent();
	}

	public void Initialize(AbstractAsset asset)
	{
		if (asset is not TextureAsset textureAsset)
			return;

		TextureName.Text = textureAsset.AssetName;
		DefaultDimensions.Content = $"{textureAsset.DefaultWidth}x{textureAsset.DefaultHeight}";
		DefaultMipmaps.Content = TextureAsset.GetMipmapCount(textureAsset.DefaultWidth, textureAsset.DefaultHeight).ToString();

		bool isPathValid = File.Exists(textureAsset.EditorPath);

		FileName.Text = isPathValid ? Path.GetFileName(textureAsset.EditorPath) : GuiUtils.FileNotFound;

		if (isPathValid)
		{
			using (SdImage image = SdImage.FromFile(textureAsset.EditorPath))
			{
				FileDimensions.Content = $"{image.Width}x{image.Height}";
				FileMipmaps.Content = TextureAsset.GetMipmapCount(image.Width, image.Height).ToString();
			}

			using FileStream imageFileStream = new(textureAsset.EditorPath, FileMode.Open, FileAccess.Read);
			MemoryStream imageCopyStream = new();
			imageFileStream.CopyTo(imageCopyStream);
			BitmapImage src = new();
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
