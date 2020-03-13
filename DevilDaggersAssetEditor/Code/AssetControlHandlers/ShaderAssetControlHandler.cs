using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ShaderAssetControlHandler : AbstractAssetControlHandler<ShaderAsset, ShaderAssetControl>
	{
		public ShaderAssetControlHandler(ShaderAsset asset, ShaderAssetControl parent)
			: base(asset, parent, "Shader files (*.glsl)|*.glsl")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}

		public override string FileNameToChunkName(string fileName) => fileName.Replace("_fragment", "").Replace("_vertex", "");
	}
}