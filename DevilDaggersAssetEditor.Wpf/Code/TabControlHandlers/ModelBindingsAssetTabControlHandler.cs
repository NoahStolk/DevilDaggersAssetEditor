using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Wpf.Code.TabControlHandlers
{
	public class ModelBindingsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>
	{
		public ModelBindingsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		protected override string AssetTypeJsonFileName => "Model Bindings";
	}
}