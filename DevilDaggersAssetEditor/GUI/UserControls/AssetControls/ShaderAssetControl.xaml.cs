using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetControls
{
	public partial class ShaderAssetControl : UserControl
	{
		public ShaderAssetControlHandler Handler { get; private set; }

		public ShaderAssetControl(ShaderAsset asset)
		{
			InitializeComponent();

			Handler = new ShaderAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e)
		{
			Handler.RemovePath();
		}

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
		{
			Handler.BrowsePath();
		}
	}
}