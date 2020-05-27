using DevilDaggersAssetCore.Assets;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	public class AssetRowEntry<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		public TAsset Asset { get; set; }
		public TAssetRowControl AssetRowControl { get; set; }
		public bool IsActive { get; set; }

		public AssetRowEntry(TAsset asset, TAssetRowControl assetRowControl, bool isActive)
		{
			Asset = asset;
			AssetRowControl = assetRowControl;
			IsActive = isActive;
		}
	}
}