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

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}