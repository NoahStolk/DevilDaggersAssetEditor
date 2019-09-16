using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetControls
{
	public partial class TextureAssetControl : UserControl
	{
		public TextureAssetControlHandler Handler { get; private set; }

		public TextureAssetControl(TextureAsset asset)
		{
			InitializeComponent();

			Handler = new TextureAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
		{
			Handler.BrowsePath();
		}
	}
}