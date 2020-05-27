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
	public partial class ParticlesAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ParticlesAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ParticlesAssetTabControlHandler Handler { get; private set; }

		private readonly AssetRowSorting<ParticleAsset, ParticleAssetRowControl> nameSort = new AssetRowSorting<ParticleAsset, ParticleAssetRowControl>((a) => a.Asset.AssetName);
		private readonly AssetRowSorting<ParticleAsset, ParticleAssetRowControl> tagsSort = new AssetRowSorting<ParticleAsset, ParticleAssetRowControl>((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting<ParticleAsset, ParticleAssetRowControl> descriptionSort = new AssetRowSorting<ParticleAsset, ParticleAssetRowControl>((a) => a.Asset.Description);
		private readonly AssetRowSorting<ParticleAsset, ParticleAssetRowControl> pathSort = new AssetRowSorting<ParticleAsset, ParticleAssetRowControl>((a) => a.Asset.EditorPath);

		public ParticlesAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ParticlesAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (ParticleAssetRowControl arc in Handler.AssetRowEntries.Select(a => a.AssetRowControl))
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
				Handler.AssetRowEntries.Select(a => new KeyValuePair<ParticleAssetRowControl, TextBlock>(a.AssetRowControl, a.AssetRowControl.Handler.TextBlockTags)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (AssetRowEntry<ParticleAsset, ParticleAssetRowControl> are in Handler.AssetRowEntries)
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
			List<AssetRowEntry<ParticleAsset, ParticleAssetRowControl>> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				ParticleAssetRowControl arc = AssetEditor.Items.OfType<ParticleAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].Asset);
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
			List<ParticleAssetRowControl> rows = AssetEditor.Items.OfType<ParticleAssetRowControl>().ToList();
			foreach (ParticleAssetRowControl row in rows)
				row.Handler.UpdateBackgroundRectangleColors(rows.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			ParticleAssetRowControl arc = e.AddedItems[0] as ParticleAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(nameSort);
		private void TagsSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(tagsSort);
		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(descriptionSort);
		private void PathSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(pathSort);

		private void SetSorting(AssetRowSorting<ParticleAsset, ParticleAssetRowControl> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}
	}

	public class ParticlesAssetTabControlHandler : AbstractAssetTabControlHandler<ParticleAsset, ParticleAssetRowControl>
	{
		protected override string AssetTypeJsonFileName => "Particles";

		public ParticlesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(ParticleAsset asset)
		{
			ParticleAssetRowControl arc = AssetRowEntries.FirstOrDefault(a => a.Asset == asset).AssetRowControl;
			arc.TextBlockEditorPath.Text = asset.EditorPath;
			arc.Handler.UpdateGui();
		}
	}
}