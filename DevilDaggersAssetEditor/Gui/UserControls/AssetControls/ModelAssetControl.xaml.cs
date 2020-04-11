using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class ModelAssetControl : UserControl
	{
		internal ModelAssetControlHandler Handler { get; private set; }

		public ModelAssetControl(ModelAsset asset)
		{
			InitializeComponent();

			Handler = new ModelAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class ModelAssetControlHandler : AbstractAssetControlHandler<ModelAsset, ModelAssetControl>
	{
		internal ModelAssetControlHandler(ModelAsset asset, ModelAssetControl parent)
			: base(asset, parent, "Model files (*.obj)|*.obj")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}