using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class ModelBindingAssetRowControl : UserControl
	{
		internal ModelBindingAssetRowControlHandler Handler { get; private set; }

		public ModelBindingAssetRowControl(ModelBindingAsset asset)
		{
			InitializeComponent();

			Handler = new ModelBindingAssetRowControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class ModelBindingAssetRowControlHandler : AbstractAssetRowControlHandler<ModelBindingAsset, ModelBindingAssetRowControl>
	{
		internal ModelBindingAssetRowControlHandler(ModelBindingAsset asset, ModelBindingAssetRowControl parent)
			: base(asset, parent, "Model binding files (*.txt)|*.txt")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}