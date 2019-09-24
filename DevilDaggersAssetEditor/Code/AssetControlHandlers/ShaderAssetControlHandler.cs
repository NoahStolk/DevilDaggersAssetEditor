using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ShaderAssetControlHandler : AbstractAssetControlHandler<ShaderAsset, ShaderAssetControl>
	{
		public ShaderAssetControlHandler(ShaderAsset asset, ShaderAssetControl parent)
			: base(asset, parent, "Shader files (*.glsl)|*.glsl")
		{
		}

		public override void UpdateGUI()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}

		public override string FileNameToChunkName(string fileName)
		{
			return fileName.Replace("_fragment", "").Replace("_vertex", "");
		}
	}
}