using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.IO;
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
			TextBlockTags.Text = string.Join(", ", asset.Tags);

			Handler = new ModelAssetRowControlHandler(asset, this, TextBlockTags, isEven);

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
		public ModelAssetRowControlHandler(ModelAsset asset, ModelAssetRowControl parent, TextBlock textBlockTags, bool isEven)
			: base(asset, parent, "Model files (*.obj)|*.obj", textBlockTags, isEven)
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			parent.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
		}
	}
}