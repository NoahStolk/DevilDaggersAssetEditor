using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class ShaderAssetControl : UserControl
	{
		internal ShaderAssetControlHandler Handler { get; private set; }

		public ShaderAssetControl(ShaderAsset asset)
		{
			InitializeComponent();

			Handler = new ShaderAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class ShaderAssetControlHandler : AbstractAssetControlHandler<ShaderAsset, ShaderAssetControl>
	{
		internal ShaderAssetControlHandler(ShaderAsset asset, ShaderAssetControl parent)
			: base(asset, parent, "Shader files (*.glsl)|*.glsl")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}

		internal override string FileNameToChunkName(string fileName) => fileName.Replace("_fragment", "").Replace("_vertex", "");
	}
}