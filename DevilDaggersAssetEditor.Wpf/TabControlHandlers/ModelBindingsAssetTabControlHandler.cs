using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class ModelBindingsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelBindingAssetRowControlHandler>
	{
		public ModelBindingsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType, AssetType.ModelBinding)
		{
		}

		protected override string AssetTypeJsonFileName => "Model Bindings";
	}
}