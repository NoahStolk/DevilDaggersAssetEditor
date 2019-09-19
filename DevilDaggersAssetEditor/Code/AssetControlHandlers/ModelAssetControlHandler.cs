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

		public override void UpdateGUI()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}