using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ModelAssetRowControl : UserControl
	{
		public ModelAssetRowControlHandler Handler { get; private set; }

		public ModelAssetRowControl(ModelAsset asset)
		{
			InitializeComponent();

			Handler = new ModelAssetRowControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ModelAssetRowControlHandler : AbstractAssetRowControlHandler<ModelAsset, ModelAssetRowControl>
	{
		public ModelAssetRowControlHandler(ModelAsset asset, ModelAssetRowControl parent)
			: base(asset, parent, "Model files (*.obj)|*.obj")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}