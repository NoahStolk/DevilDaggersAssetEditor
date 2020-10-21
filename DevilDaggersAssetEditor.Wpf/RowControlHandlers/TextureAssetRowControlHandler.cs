using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Wpf.RowControlHandlers
{
	public class TextureAssetRowControlHandler : AssetRowControlHandler
	{
		public TextureAssetRowControlHandler(AbstractAsset asset, bool isEven)
			: base(asset, AssetType.Texture, isEven, "Texture files (*.png)|*.png")
		{
		}
	}
}