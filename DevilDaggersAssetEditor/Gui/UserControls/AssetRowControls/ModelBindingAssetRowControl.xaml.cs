using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ModelBindingAssetRowControl : UserControl
	{
		public ModelBindingAssetRowControlHandler Handler { get; private set; }

		public ModelBindingAssetRowControl(ModelBindingAsset asset, bool isEven)
		{
			InitializeComponent();
			TextBlockTags.Text = asset.Tags != null ? string.Join(", ", asset.Tags) : string.Empty;

			Handler = new ModelBindingAssetRowControlHandler(asset, this, TextBlockTags, isEven);

			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ModelBindingAssetRowControlHandler : AbstractAssetRowControlHandler<ModelBindingAsset, ModelBindingAssetRowControl>
	{
		public ModelBindingAssetRowControlHandler(ModelBindingAsset asset, ModelBindingAssetRowControl parent, TextBlock textBlockTags, bool isEven)
			: base(asset, parent, "Model binding files (*.txt)|*.txt", textBlockTags, isEven)
		{
		}

		public override void UpdateGui()
		{
			bool isPathValid = File.Exists(Asset.EditorPath);
			parent.TextBlockEditorPath.Text = isPathValid ? Asset.EditorPath : Utils.FileNotFound;
		}
	}
}