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
	public partial class ModelsAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ModelsAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ModelsAssetTabControlHandler Handler { get; private set; }

		private readonly AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler> nameSort = new AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.AssetName);
		private readonly AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler> tagsSort = new AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler>((a) => string.Join(", ", a.AssetRowControlHandler.Asset.Tags));
		private readonly AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler> descriptionSort = new AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.Description);
		private readonly AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler> pathSort = new AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.EditorPath);

		public ModelsAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ModelsAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (ModelAssetRowControl arc in Handler.AssetRowEntries.Select(a => a.AssetRowControlHandler.AssetRowControl))
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
				Handler.AssetRowEntries.Select(a => new KeyValuePair<ModelAssetRowControl, TextBlock>(a.AssetRowControlHandler.AssetRowControl, a.AssetRowControlHandler.AssetRowControl.Handler.TextBlockTags)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (AssetRowEntry<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler> assetRowEntry in Handler.AssetRowEntries)
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
			List<AssetRowEntry<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler>> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				ModelAssetRowControl arc = AssetEditor.Items.OfType<ModelAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].AssetRowControlHandler.Asset);
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
			foreach (ModelAssetRowControl row in AssetEditor.Items)
				row.Handler.UpdateBackgroundRectangleColors(AssetEditor.Items.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			ModelAssetRowControl arc = e.AddedItems[0] as ModelAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(nameSort);
		private void TagsSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(tagsSort);
		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(descriptionSort);
		private void PathSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(pathSort);

		private void SetSorting(AssetRowSorting<ModelAsset, ModelAssetRowControl, ModelAssetRowControlHandler> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => Handler?.UpdateTagHighlighting();
	}
}