using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class ModelBindingAssetControl : UserControl
	{
		public ModelBindingAssetControlHandler Handler { get; private set; }

		public ModelBindingAssetControl(ModelBindingAsset asset)
		{
			InitializeComponent();

			Handler = new ModelBindingAssetControlHandler(asset, this);

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