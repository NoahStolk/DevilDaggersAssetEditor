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

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}