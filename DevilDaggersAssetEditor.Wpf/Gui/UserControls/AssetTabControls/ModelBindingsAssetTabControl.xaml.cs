using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.TabControlHandlers;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetTabControls
{
	public partial class ModelBindingsAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register(nameof(BinaryFileType), typeof(string), typeof(ModelBindingsAssetTabControl));

		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> _nameSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => a.Asset.AssetName);
		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> _tagsSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> _descriptionSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => a.Asset.Description);
		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> _pathSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => a.Asset.EditorPath);

		public ModelBindingsAssetTabControl()
		{
			InitializeComponent();
		}

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ModelBindingsAssetTabControlHandler Handler { get; private set; }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ModelBindingsAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (ModelBindingAssetRowControl arc in Handler.RowHandlers.Select(a => a.AssetRowControl))
				AssetEditor.Items.Add(arc);

			CreateFiltersGui();
		}

		private void CreateFiltersGui()
		{
			FilterOperationAnd.Checked += ApplyFilter;
			FilterOperationOr.Checked += ApplyFilter;

			int columns = 9;
			int rows = (int)Math.Ceiling(Handler.FiltersCount / (double)columns);
			Handler.CreateFiltersGui(rows);

			for (int i = 0; i < columns; i++)
				Filters.ColumnDefinitions.Add(new ColumnDefinition());
			for (int i = 0; i < rows; i++)
				Filters.RowDefinitions.Add(new RowDefinition());

			foreach (CheckBox checkBox in Handler.FilterCheckBoxes)
			{
				checkBox.Checked += ApplyFilter;
				checkBox.Unchecked += ApplyFilter;
				Filters.Children.Add(checkBox);
			}
		}

		private void ApplyFilter(object sender, RoutedEventArgs e)
		{
			Handler.ApplyFilter(GetFilterOperation());

			foreach (ModelBindingAssetRowControlHandler assetRowEntry in Handler.RowHandlers)
			{
				if (!assetRowEntry.IsActive)
				{
					if (AssetEditor.Items.Contains(assetRowEntry.AssetRowControl))
						AssetEditor.Items.Remove(assetRowEntry.AssetRowControl);
				}
				else
				{
					if (!AssetEditor.Items.Contains(assetRowEntry.AssetRowControl))
						AssetEditor.Items.Add(assetRowEntry.AssetRowControl);
				}
			}

			ApplySort();
		}

		private void ApplySort()
		{
			List<ModelBindingAssetRowControlHandler> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				ModelBindingAssetRowControl arc = AssetEditor.Items.OfType<ModelBindingAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].Asset);
				AssetEditor.Items.Remove(arc);
				AssetEditor.Items.Insert(i, arc);
			}

			Handler.SetAssetEditorBackgroundColors(AssetEditor.Items);
		}

		private FilterOperation GetFilterOperation()
		{
			if (FilterOperationAnd.IsChecked())
				return FilterOperation.And;
			if (FilterOperationOr.IsChecked())
				return FilterOperation.Or;
			return FilterOperation.None;
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			ModelBindingAssetRowControl arc = e.AddedItems[0] as ModelBindingAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_nameSort);

		private void TagsSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_tagsSort);

		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_descriptionSort);

		private void PathSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_pathSort);

		private void SetSorting(AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> Handler?.UpdateTagHighlighting();
	}
}