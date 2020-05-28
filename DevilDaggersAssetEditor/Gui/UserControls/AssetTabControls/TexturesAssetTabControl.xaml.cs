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
	public partial class TexturesAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(TexturesAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public TexturesAssetTabControlHandler Handler { get; private set; }

		private readonly AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler> nameSort = new AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.AssetName);
		private readonly AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler> tagsSort = new AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>((a) => string.Join(", ", a.AssetRowControlHandler.Asset.Tags));
		private readonly AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler> descriptionSort = new AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.Description);
		private readonly AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler> pathSort = new AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.EditorPath);

		public TexturesAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new TexturesAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (TextureAssetRowControl arc in Handler.AssetRowEntries.Select(a => a.AssetRowControlHandler.AssetRowControl))
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
				Handler.AssetRowEntries.Select(a => new KeyValuePair<TextureAssetRowControl, TextBlock>(a.AssetRowControlHandler.AssetRowControl, a.AssetRowControlHandler.AssetRowControl.Handler.TextBlockTags)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (AssetRowEntry<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler> are in Handler.AssetRowEntries)
			{
				if (!are.IsActive)
				{
					if (AssetEditor.Items.Contains(are.AssetRowControlHandler.AssetRowControl))
						AssetEditor.Items.Remove(are.AssetRowControlHandler.AssetRowControl);
				}
				else
				{
					if (!AssetEditor.Items.Contains(are.AssetRowControlHandler.AssetRowControl))
						AssetEditor.Items.Add(are.AssetRowControlHandler.AssetRowControl);
				}
			}

			ApplySort();
		}

		private void ApplySort()
		{
			List<AssetRowEntry<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				TextureAssetRowControl arc = AssetEditor.Items.OfType<TextureAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].AssetRowControlHandler.Asset);
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
			foreach (TextureAssetRowControl row in AssetEditor.Items)
				row.Handler.UpdateBackgroundRectangleColors(AssetEditor.Items.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			TextureAssetRowControl arc = e.AddedItems[0] as TextureAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(nameSort);
		private void TagsSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(tagsSort);
		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(descriptionSort);
		private void PathSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(pathSort);

		private void SetSorting(AssetRowSorting<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => Handler?.UpdateTagHighlighting();
	}
}