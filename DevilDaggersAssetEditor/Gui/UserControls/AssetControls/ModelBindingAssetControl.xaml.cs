using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class ModelBindingAssetControl : UserControl
	{
		internal ModelBindingAssetControlHandler Handler { get; private set; }

		public ModelBindingAssetControl(ModelBindingAsset asset)
		{
			InitializeComponent();

			Handler = new ModelBindingAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class ModelBindingAssetControlHandler : AbstractAssetControlHandler<ModelBindingAsset, ModelBindingAssetControl>
	{
		internal ModelBindingAssetControlHandler(ModelBindingAsset asset, ModelBindingAssetControl parent)
			: base(asset, parent, "Model binding files (*.txt)|*.txt")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}