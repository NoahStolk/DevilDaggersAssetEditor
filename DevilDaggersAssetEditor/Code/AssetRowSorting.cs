using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using System;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	public class AssetRowSorting<TAsset, TAssetRowControl, TAssetRowControlHandler>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
		where TAssetRowControlHandler : AbstractAssetRowControlHandler<TAsset, TAssetRowControl>
	{
		public Func<AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>, object> SortingFunction { get; set; }
		public bool IsAscending { get; set; } = true;

		public AssetRowSorting(Func<AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>, object> sortingFunction)
		{
			SortingFunction = sortingFunction;
		}
	}
}