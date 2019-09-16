using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ModelBindingAssetControlHandler : AbstractAssetControlHandler<ModelBindingAsset, ModelBindingAssetControl>
	{
		public ModelBindingAssetControlHandler(ModelBindingAsset asset, ModelBindingAssetControl modelBindingAssetControl)
			: base(asset, modelBindingAssetControl, "Text files (*.txt)|*.txt")
		{
		}

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.LabelEditorPath.Content = Asset.EditorPath;
		}
	}
}