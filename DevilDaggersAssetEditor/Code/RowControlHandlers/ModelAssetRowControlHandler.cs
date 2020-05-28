using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.IO;

namespace DevilDaggersAssetEditor.Code.RowControlHandlers
{
	public class ModelAssetRowControlHandler : AbstractAssetRowControlHandler<ModelAsset, ModelAssetRowControl>
	{
		public override string OpenDialogFilter => "Model files (*.obj)|*.obj";

		public ModelAssetRowControlHandler(ModelAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
		}
	}
}