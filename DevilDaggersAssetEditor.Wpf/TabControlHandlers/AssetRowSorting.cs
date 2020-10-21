using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using System;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class AssetRowSorting<TAssetRowControlHandler>
		where TAssetRowControlHandler : AssetRowControlHandler
	{
		public AssetRowSorting(Func<TAssetRowControlHandler, object> sortingFunction)
		{
			SortingFunction = sortingFunction;
		}

		public Func<TAssetRowControlHandler, object> SortingFunction { get; set; }
		public bool IsAscending { get; set; } = true;
	}
}