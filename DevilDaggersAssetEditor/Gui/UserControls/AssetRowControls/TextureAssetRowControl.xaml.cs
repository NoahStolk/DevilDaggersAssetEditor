using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class TextureAssetRowControl : UserControl
	{
		public TextureAssetRowControlHandler Handler { get; private set; }

		public TextureAssetRowControl(TextureAsset asset)
		{
			InitializeComponent();

			Handler = new TextureAssetRowControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class TextureAssetRowControlHandler : AbstractAssetRowControlHandler<TextureAsset, TextureAssetRowControl>
	{
		public TextureAssetRowControlHandler(TextureAsset asset, TextureAssetRowControl parent)
			: base(asset, parent, "Texture files (*.png)|*.png")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}