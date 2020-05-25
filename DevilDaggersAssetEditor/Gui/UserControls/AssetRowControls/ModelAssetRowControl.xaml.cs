using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ModelAssetRowControl : UserControl
	{
		public ModelAssetRowControlHandler Handler { get; private set; }

		public ModelAssetRowControl(ModelAsset asset, bool isEven)
		{
			InitializeComponent();
			TextBlockTags.Text = asset.Tags != null ? string.Join(", ", asset.Tags) : string.Empty;

			Handler = new ModelAssetRowControlHandler(asset, this, isEven);

			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ModelAssetRowControlHandler : AbstractAssetRowControlHandler<ModelAsset, ModelAssetRowControl>
	{
		public ModelAssetRowControlHandler(ModelAsset asset, ModelAssetRowControl parent, bool isEven)
			: base(asset, parent, "Model files (*.obj)|*.obj", isEven)
		{
		}

		public override void UpdateGui()
		{
			bool isPathValid = Asset.EditorPath.GetPathValidity() == PathValidity.Valid;
			parent.TextBlockEditorPath.Text = isPathValid ? Asset.EditorPath : Utils.GetPathValidityMessage(Asset.EditorPath);
		}
	}
}