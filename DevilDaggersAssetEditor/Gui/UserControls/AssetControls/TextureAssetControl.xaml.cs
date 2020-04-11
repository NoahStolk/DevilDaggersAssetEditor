using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class TextureAssetControl : UserControl
	{
		internal TextureAssetControlHandler Handler { get; private set; }

		public TextureAssetControl(TextureAsset asset)
		{
			InitializeComponent();

			Handler = new TextureAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class TextureAssetControlHandler : AbstractAssetControlHandler<TextureAsset, TextureAssetControl>
	{
		internal TextureAssetControlHandler(TextureAsset asset, TextureAssetControl parent)
			: base(asset, parent, "Texture files (*.png)|*.png")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}