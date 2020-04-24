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

		private void ButtonVertexRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonVertexBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();

		private void ButtonFragmentRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonFragmentBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ShaderAssetRowControlHandler : AbstractAssetRowControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		public ShaderAssetRowControlHandler(ShaderAsset asset, ShaderAssetRowControl parent)
			: base(asset, parent, "Shader files (*.glsl)|*.glsl")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockVertexEditorPath.Text = Asset.EditorPath; // TODO
			parent.TextBlockFragmentEditorPath.Text = Asset.EditorPath; // TODO
		}

		public override string FileNameToChunkName(string fileName) => fileName.Replace("_fragment", "").Replace("_vertex", "");
	}
}