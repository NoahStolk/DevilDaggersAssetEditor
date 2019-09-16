using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ModelAssetControlHandler : AbstractAssetControlHandler<ModelAsset, ModelAssetControl>
	{
		public ModelAssetControlHandler(ModelAsset asset, ModelAssetControl parent)
			: base(asset, parent, "Model files (*.obj)|*.obj")
		{
		}

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}