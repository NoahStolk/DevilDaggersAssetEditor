using DevilDaggersAssetCore.Assets;
using System;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	public class AssetRowSorting<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		public Func<AssetRowEntry<TAsset, TAssetRowControl>, object> SortingFunction { get; set; }
		public bool IsAscending { get; set; } = true;

		public AssetRowSorting(Func<AssetRowEntry<TAsset, TAssetRowControl>, object> sortingFunction)
		{
			SortingFunction = sortingFunction;
		}
	}
}