using System;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls
{
	public class AssetRowSorting
	{
		public AssetRowSorting(Func<AssetRowControl, object> sortingFunction)
		{
			SortingFunction = sortingFunction;
		}

		public Func<AssetRowControl, object> SortingFunction { get; set; }
		public bool IsAscending { get; set; } = true;
	}
}