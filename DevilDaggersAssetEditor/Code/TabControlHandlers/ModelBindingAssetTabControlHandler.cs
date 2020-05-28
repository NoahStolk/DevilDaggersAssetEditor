using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ModelBindingsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Model Bindings";

		public ModelBindingsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(ModelBindingAsset asset)
		{
			ModelBindingAssetRowControl arc = AssetRowEntries.FirstOrDefault(a => a.AssetRowControlHandler.Asset == asset).AssetRowControlHandler.AssetRowControl;
			arc.Handler.UpdateGui();
		}
	}
}