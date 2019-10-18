using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetControls
{
	public partial class ModelAssetControl : UserControl
	{
		public ModelAssetControlHandler Handler { get; private set; }

		public ModelAssetControl(ModelAsset asset)
		{
			InitializeComponent();

			Handler = new ModelAssetControlHandler(asset, this);

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