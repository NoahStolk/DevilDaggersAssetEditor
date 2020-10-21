using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using System;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class AssetRowSorting
	{
		public AssetRowSorting(Func<AssetRowControlHandler, object> sortingFunction)
		{
			SortingFunction = sortingFunction;
		}

		public Func<AssetRowControlHandler, object> SortingFunction { get; set; }
		public bool IsAscending { get; set; } = true;
	}
}