using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers
{
	public class ShaderAssetRowControlHandler : AbstractAssetRowControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		public ShaderAssetRowControlHandler(ShaderAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override string OpenDialogFilter => "Shader files (*.glsl)|*.glsl";

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockVertexEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture)) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_vertex").TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
			AssetRowControl.TextBlockFragmentEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture)) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_fragment").TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
		}
	}
}