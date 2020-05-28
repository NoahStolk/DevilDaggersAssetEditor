using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.IO;

namespace DevilDaggersAssetEditor.Code.RowControlHandlers
{
	public class ShaderAssetRowControlHandler : AbstractAssetRowControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		public override string OpenDialogFilter => "Shader files (*.glsl)|*.glsl";

		public ShaderAssetRowControlHandler(ShaderAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockVertexEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_vertex.glsl")) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_vertex").TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
			AssetRowControl.TextBlockFragmentEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_fragment.glsl")) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_fragment").TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
		}
	}
}