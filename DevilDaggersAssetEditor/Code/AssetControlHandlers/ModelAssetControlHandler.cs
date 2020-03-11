using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ModelAssetControlHandler : AbstractAssetControlHandler<ModelAsset, ModelAssetControl>
	{
		public ModelAssetControlHandler(ModelAsset asset, ModelAssetControl parent)
			: base(asset, parent, "Model files (*.obj)|*.obj")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}