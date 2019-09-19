using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ModelBindingAssetControlHandler : AbstractAssetControlHandler<ModelBindingAsset, ModelBindingAssetControl>
	{
		public ModelBindingAssetControlHandler(ModelBindingAsset asset, ModelBindingAssetControl parent)
			: base(asset, parent, "Model binding files (*.txt)|*.txt")
		{
		}

		public override void UpdateGUI()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}