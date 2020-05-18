using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ShaderAssetRowControl : UserControl
	{
		public ShaderAssetRowControlHandler Handler { get; private set; }

		public ShaderAssetRowControl(ShaderAsset asset)
		{
			InitializeComponent();

			Handler = new ShaderAssetRowControlHandler(asset, this);

			Data.DataContext = asset;

			TextBlockVertexName.Text = $"{asset.AssetName}_vertex";
			TextBlockFragmentName.Text = $"{asset.AssetName}_fragment";
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ShaderAssetRowControlHandler : AbstractAssetRowControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		public ShaderAssetRowControlHandler(ShaderAsset asset, ShaderAssetRowControl parent)
			: base(asset, parent, "Shader files (*.glsl)|*.glsl")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockVertexEditorPath.Text = Asset.EditorPath.IsPathValid() ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_vertex") : Asset.EditorPath;
			parent.TextBlockFragmentEditorPath.Text = Asset.EditorPath.IsPathValid() ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_fragment") : Asset.EditorPath;
		}

		public override string FileNameToChunkName(string fileName) => fileName.Replace("_fragment", "").Replace("_vertex", "");
	}
}