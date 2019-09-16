using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ModelBindingsTabControlHandler : AbstractTabControlHandler<ModelBindingAsset, ModelBindingAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Model Bindings";

		public ModelBindingsTabControlHandler(BinaryFileName binaryFileName)
			: base(binaryFileName)
		{
		}

		protected override void UpdatePathLabel(ModelBindingAsset asset)
		{
			ModelBindingAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}