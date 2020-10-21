using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Wpf.RowControlHandlers
{
	public class ModelBindingAssetRowControlHandler : AssetRowControlHandler
	{
		public ModelBindingAssetRowControlHandler(AbstractAsset asset, bool isEven)
			: base(asset, AssetType.ModelBinding, isEven, "Model binding files (*.txt)|*.txt")
		{
		}
	}
}