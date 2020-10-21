using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Wpf.RowControlHandlers
{
	public class ShaderAssetRowControlHandler : AssetRowControlHandler
	{
		public ShaderAssetRowControlHandler(AbstractAsset asset, bool isEven)
			: base(asset, AssetType.Shader, isEven, "Shader files (*.glsl)|*.glsl")
		{
		}

		public override void UpdateGui()
		{
			base.UpdateGui();

			// AssetRowControl.TextBlockVertexEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture)) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_vertex").TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
			// AssetRowControl.TextBlockFragmentEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture)) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_fragment").TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
		}
	}
}