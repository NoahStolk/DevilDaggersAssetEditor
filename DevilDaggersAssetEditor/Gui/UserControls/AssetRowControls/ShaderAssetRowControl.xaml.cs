using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ShaderAssetRowControl : UserControl
	{
		public ShaderAssetRowControlHandler Handler { get; private set; }

		public ShaderAssetRowControl(ShaderAsset asset, bool isEven)
		{
			InitializeComponent();
			TextBlockTags.Text = asset.Tags != null ? string.Join(", ", asset.Tags) : string.Empty;

			Handler = new ShaderAssetRowControlHandler(asset, this, TextBlockTags, isEven);

			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = asset;

			TextBlockVertexName.Text = $"{asset.AssetName}_vertex";
			TextBlockFragmentName.Text = $"{asset.AssetName}_fragment";
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ShaderAssetRowControlHandler : AbstractAssetRowControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		public ShaderAssetRowControlHandler(ShaderAsset asset, ShaderAssetRowControl parent, TextBlock textBlockTags, bool isEven)
			: base(asset, parent, "Shader files (*.glsl)|*.glsl", textBlockTags, isEven)
		{
		}

		public override void UpdateGui()
		{
			bool isVertexPathValid = File.Exists(Asset.EditorPath.Replace(".glsl", "_vertex.glsl"));
			bool isFragmentPathValid = File.Exists(Asset.EditorPath.Replace(".glsl", "_fragment.glsl"));
			parent.TextBlockVertexEditorPath.Text = isVertexPathValid ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_vertex") : Utils.FileNotFound;
			parent.TextBlockFragmentEditorPath.Text = isFragmentPathValid ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_fragment") : Utils.FileNotFound;
		}

		public override string FileNameToChunkName(string fileName) => fileName.Replace("_fragment", "").Replace("_vertex", "");
	}
}