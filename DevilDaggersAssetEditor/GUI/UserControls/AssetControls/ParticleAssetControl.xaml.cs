using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetControls
{
	public partial class ParticleAssetControl : UserControl
	{
		public ParticleAssetControlHandler Handler { get; private set; }

		public ParticleAssetControl(ParticleAsset asset)
		{
			InitializeComponent();

			Handler = new ParticleAssetControlHandler(asset, this);

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