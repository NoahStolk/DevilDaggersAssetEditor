using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetTabControls
{
	public partial class ShadersAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ShadersAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ShadersAssetTabControlHandler Handler { get; private set; }

		private readonly AssetRowSorting<ShaderAsset, ShaderAssetRowControl> nameSort = new AssetRowSorting<ShaderAsset, ShaderAssetRowControl>((a) => a.Asset.AssetName);
		private readonly AssetRowSorting<ShaderAsset, ShaderAssetRowControl> tagsSort = new AssetRowSorting<ShaderAsset, ShaderAssetRowControl>((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting<ShaderAsset, ShaderAssetRowControl> descriptionSort = new AssetRowSorting<ShaderAsset, ShaderAssetRowControl>((a) => a.Asset.Description);
		private readonly AssetRowSorting<ShaderAsset, ShaderAssetRowControl> pathSort = new AssetRowSorting<ShaderAsset, ShaderAssetRowControl>((a) => a.Asset.EditorPath);

		public ShadersAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ShadersAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (ShaderAssetRowControl arc in Handler.AssetRowEntries.Select(a => a.AssetRowControl))
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
				Handler.AssetRowEntries.Select(a => new KeyValuePair<ShaderAssetRowControl, TextBlock>(a.AssetRowControl, a.AssetRowControl.Handler.TextBlockTags)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (AssetRowEntry<ShaderAsset, ShaderAssetRowControl> are in Handler.AssetRowEntries)
			{
				if (!are.IsActive)
				{
					if (AssetEditor.Items.Contains(are.AssetRowControl))
						AssetEditor.Items.Remove(are.AssetRowControl);
				}
				else
				{
					if (!AssetEditor.Items.Contains(are.AssetRowControl))
						AssetEditor.Items.Add(are.AssetRowControl);
				}
			}

			ApplySort();
		}

		private void ApplySort()
		{
			List<AssetRowEntry<ShaderAsset, ShaderAssetRowControl>> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				ShaderAssetRowControl arc = AssetEditor.Items.OfType<ShaderAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].Asset);
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
			List<ShaderAssetRowControl> rows = AssetEditor.Items.OfType<ShaderAssetRowControl>().ToList();
			foreach (ShaderAssetRowControl row in rows)
				row.Handler.UpdateBackgroundRectangleColors(rows.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			ShaderAssetRowControl arc = e.AddedItems[0] as ShaderAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(nameSort);
		private void TagsSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(tagsSort);
		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(descriptionSort);
		private void PathSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(pathSort);

		private void SetSorting(AssetRowSorting<ShaderAsset, ShaderAssetRowControl> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}
	}

	public class ShadersAssetTabControlHandler : AbstractAssetTabControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		protected override string AssetTypeJsonFileName => "Shaders";

		public ShadersAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(ShaderAsset asset)
		{
			ShaderAssetRowControl arc = AssetRowEntries.FirstOrDefault(a => a.Asset == asset).AssetRowControl;
			arc.TextBlockVertexEditorPath.Text = asset.EditorPath;
			arc.TextBlockFragmentEditorPath.Text = asset.EditorPath;
			arc.Handler.UpdateGui();
		}
	}
}