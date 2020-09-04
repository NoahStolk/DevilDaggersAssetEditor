using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Wpf.Code.TabControlHandlers
{
	public class ModelsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler>
	{
		public ModelsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		protected override string AssetTypeJsonFileName => "Models";
	}
}