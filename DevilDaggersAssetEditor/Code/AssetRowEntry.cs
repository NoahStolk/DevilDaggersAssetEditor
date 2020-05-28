using DevilDaggersAssetCore.Assets;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	public class AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
		where TAssetRowControlHandler : AbstractAssetRowControlHandler<TAsset, TAssetRowControl>
	{
		public TAssetRowControlHandler AssetRowControlHandler { get; set; }
		public bool IsActive { get; set; }

		public AssetRowEntry(TAssetRowControlHandler assetRowControlHandler, bool isActive)
		{
			AssetRowControlHandler = assetRowControlHandler;
			IsActive = isActive;
		}

		public override string ToString() => $"{AssetRowControlHandler.Asset.AssetName} {(IsActive ? "" : "(Inactive)")}";
	}
}