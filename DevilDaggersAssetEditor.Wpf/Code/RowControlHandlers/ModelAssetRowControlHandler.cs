using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers
{
	public class ModelAssetRowControlHandler : AbstractAssetRowControlHandler<ModelAsset, ModelAssetRowControl>
	{
		public ModelAssetRowControlHandler(ModelAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override string OpenDialogFilter => "Model files (*.obj)|*.obj";

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
		}
	}
}