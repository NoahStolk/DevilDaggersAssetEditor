using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ModelBindingAssetRowControl : UserControl
	{
		public ModelBindingAssetRowControlHandler Handler { get; private set; }

		public ModelBindingAssetRowControl(ModelBindingAsset asset)
		{
			InitializeComponent();

			Handler = new ModelBindingAssetRowControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ModelBindingAssetRowControlHandler : AbstractAssetRowControlHandler<ModelBindingAsset, ModelBindingAssetRowControl>
	{
		public ModelBindingAssetRowControlHandler(ModelBindingAsset asset, ModelBindingAssetRowControl parent)
			: base(asset, parent, "Model binding files (*.txt)|*.txt")
		{
		}

		public override void UpdateGui()
		{
			bool isPathValid = Asset.EditorPath.GetPathValidity() == PathValidity.Valid;
			parent.TextBlockEditorPath.Text = isPathValid ? Asset.EditorPath : Utils.GetPathValidityMessage(Asset.EditorPath);
		}
	}
}