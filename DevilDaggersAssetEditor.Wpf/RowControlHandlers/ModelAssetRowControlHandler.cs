using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Wpf.RowControlHandlers
{
	public class ModelAssetRowControlHandler : AssetRowControlHandler
	{
		public ModelAssetRowControlHandler(AbstractAsset asset, bool isEven)
			: base(asset, AssetType.Model, isEven, "Model files (*.obj)|*.obj")
		{
		}
	}
}