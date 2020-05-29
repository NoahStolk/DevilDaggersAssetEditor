using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ModelBindingsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Model Bindings";

		public ModelBindingsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}
	}
}