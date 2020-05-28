using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetTabControls
{
	public partial class ModelBindingsAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ModelBindingsAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ModelBindingsAssetTabControlHandler Handler { get; private set; }

		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> nameSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.AssetName);
		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> tagsSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => string.Join(", ", a.AssetRowControlHandler.Asset.Tags));
		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> descriptionSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.Description);
		private readonly AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> pathSort = new AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.EditorPath);

		public ModelBindingsAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ModelBindingsAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (ModelBindingAssetRowControl arc in Handler.AssetRowEntries.Select(a => a.AssetRowControlHandler.AssetRowControl))
				AssetEditor.Items.Add(arc);

			CreateFiltersGui();
		}

		private void CreateFiltersGui()
		{
			FilterOperationAnd.Checked += ApplyFilter;
			FilterOperationOr.Checked += ApplyFilter;

			Handler.CreateFiltersGui();

			foreach (StackPanel stackPanel in Handler.filterStackPanels)
			{
				Filters.ColumnDefinitions.Add(new ColumnDefinition());
				Filters.Children.Add(stackPanel);
			}

			foreach (CheckBox checkBox in Handler.filterCheckBoxes)
			{
				checkBox.Checked += ApplyFilter;
				checkBox.Unchecked += ApplyFilter;
			}
		}

		private void ApplyFilter(object sender, RoutedEventArgs e)
		{
			Handler.ApplyFilter(
				GetFilterOperation(),
				Handler.AssetRowEntries.Select(a => new KeyValuePair<ModelBindingAssetRowControl, TextBlock>(a.AssetRowControlHandler.AssetRowControl, a.AssetRowControlHandler.AssetRowControl.Handler.TextBlockTags)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (AssetRowEntry<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> assetRowEntry in Handler.AssetRowEntries)
			{
				if (!assetRowEntry.IsActive)
				{
					if (AssetEditor.Items.Contains(assetRowEntry.AssetRowControlHandler.AssetRowControl))
						AssetEditor.Items.Remove(assetRowEntry.AssetRowControlHandler.AssetRowControl);
				}
				else
				{
					if (!AssetEditor.Items.Contains(assetRowEntry.AssetRowControlHandler.AssetRowControl))
						AssetEditor.Items.Add(assetRowEntry.AssetRowControlHandler.AssetRowControl);
				}
			}

			ApplySort();
		}

		private void ApplySort()
		{
			List<AssetRowEntry<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler>> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				ModelBindingAssetRowControl arc = AssetEditor.Items.OfType<ModelBindingAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].AssetRowControlHandler.Asset);
				AssetEditor.Items.Remove(arc);
				AssetEditor.Items.Insert(i, arc);
			}

			SetAssetEditorBackgroundColors();
		}

		private FilterOperation GetFilterOperation()
		{
			if (FilterOperationAnd.IsChecked.Value)
				return FilterOperation.And;
			if (FilterOperationOr.IsChecked.Value)
				return FilterOperation.Or;
			return FilterOperation.None;
		}

		private void SetAssetEditorBackgroundColors()
		{
			foreach (ModelBindingAssetRowControl row in AssetEditor.Items)
				row.Handler.UpdateBackgroundRectangleColors(AssetEditor.Items.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			ModelBindingAssetRowControl arc = e.AddedItems[0] as ModelBindingAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(nameSort);
		private void TagsSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(tagsSort);
		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(descriptionSort);
		private void PathSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(pathSort);

		private void SetSorting(AssetRowSorting<ModelBindingAsset, ModelBindingAssetRowControl, ModelBindingAssetRowControlHandler> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => Handler?.UpdateTagHighlighting();
	}
}