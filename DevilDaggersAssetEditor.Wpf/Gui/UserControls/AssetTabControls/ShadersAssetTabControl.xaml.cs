using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetTabControls
{
	public partial class ShadersAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register(nameof(BinaryFileType), typeof(string), typeof(ShadersAssetTabControl));

		private readonly AssetRowSorting _nameSort = new AssetRowSorting((a) => a.Asset.AssetName);
		private readonly AssetRowSorting _tagsSort = new AssetRowSorting((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting _descriptionSort = new AssetRowSorting((a) => a.Asset.Description);
		private readonly AssetRowSorting _pathSort = new AssetRowSorting((a) => a.Asset.EditorPath);

		public ShadersAssetTabControl()
		{
			InitializeComponent();
		}

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AssetTabControlHandler Handler { get; private set; }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true), Assets.AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");

			foreach (AssetRowControl arc in Handler.RowHandlers.Select(a => a.AssetRowControl))
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

			foreach (AssetRowControlHandler assetRowEntry in Handler.RowHandlers)
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
			List<AssetRowControlHandler> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				ShaderAssetRowControl arc = AssetEditor.Items.OfType<ShaderAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].Asset);
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

			ShaderAssetRowControl arc = e.AddedItems[0] as ShaderAssetRowControl;

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

		private void SetSorting(AssetRowSorting sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> Handler?.UpdateTagHighlighting();
	}
}