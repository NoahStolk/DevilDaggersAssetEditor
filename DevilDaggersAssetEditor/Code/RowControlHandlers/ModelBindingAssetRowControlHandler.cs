using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.IO;

namespace DevilDaggersAssetEditor.Code.RowControlHandlers
{
	public class ModelBindingAssetRowControlHandler : AbstractAssetRowControlHandler<ModelBindingAsset, ModelBindingAssetRowControl>
	{
		public override string OpenDialogFilter => "Model binding files (*.txt)|*.txt";

		public ModelBindingAssetRowControlHandler(ModelBindingAsset asset, bool isEven)
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