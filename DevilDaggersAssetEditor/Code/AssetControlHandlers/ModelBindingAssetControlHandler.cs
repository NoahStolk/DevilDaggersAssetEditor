using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ModelBindingAssetControlHandler : AbstractAssetControlHandler<ModelBindingAsset, ModelBindingAssetControl>
	{
		public ModelBindingAssetControlHandler(ModelBindingAsset asset, ModelBindingAssetControl parent)
			: base(asset, parent, "Model binding files (*.txt)|*.txt")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}