using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetControls
{
	public partial class AudioAssetControl : UserControl
	{
		public AudioAssetControlHandler Handler { get; private set; }

		public AudioAssetControl(AudioAsset asset)
		{
			InitializeComponent();

			Handler = new AudioAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
		{
			Handler.BrowsePath();
		}
	}
}