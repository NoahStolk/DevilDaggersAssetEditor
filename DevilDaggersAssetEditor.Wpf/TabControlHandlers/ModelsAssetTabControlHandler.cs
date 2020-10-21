using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class ModelsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelAssetRowControlHandler>
	{
		public ModelsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType, AssetType.Model)
		{
		}

		protected override string AssetTypeJsonFileName => "Models";
	}
}