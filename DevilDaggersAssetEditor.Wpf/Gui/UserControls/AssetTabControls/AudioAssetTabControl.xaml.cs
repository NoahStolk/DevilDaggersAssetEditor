using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.TabControlHandlers;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetTabControls
{
	public partial class AudioAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register(
			nameof(BinaryFileType),
			typeof(string),
			typeof(AudioAssetTabControl));

		private readonly AssetRowSorting<AudioAssetRowControlHandler> _nameSort = new AssetRowSorting<AudioAssetRowControlHandler>((a) => a.Asset.AssetName);
		private readonly AssetRowSorting<AudioAssetRowControlHandler> _tagsSort = new AssetRowSorting<AudioAssetRowControlHandler>((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting<AudioAssetRowControlHandler> _descriptionSort = new AssetRowSorting<AudioAssetRowControlHandler>((a) => a.Asset.Description);
		private readonly AssetRowSorting<AudioAssetRowControlHandler> _loudnessSort = new AssetRowSorting<AudioAssetRowControlHandler>((a) => (a.Asset as AudioAsset).Loudness);
		private readonly AssetRowSorting<AudioAssetRowControlHandler> _pathSort = new AssetRowSorting<AudioAssetRowControlHandler>((a) => a.Asset.EditorPath);

		public AudioAssetTabControl()
		{
			InitializeComponent();

			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
			timer.Tick += (sender, e) =>
			{
				if (Previewer.Song == null || Previewer.Song.Paused)
					return;

				if (!Previewer.IsDragging)
				{
					float length = Previewer.Song.PlayLength;
					if (length == 0)
						length = 1;
					Previewer.Seek.Value = Previewer.Song.PlayPosition / length * Previewer.Seek.Maximum;
				}

				Previewer.SeekText.Content = $"{EditorUtils.ToTimeString((int)Previewer.Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Previewer.Song.PlayLength)}";
			};
			timer.Start();
		}

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AudioAssetTabControlHandler Handler { get; private set; }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

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

			foreach (AudioAssetRowControlHandler assetRowEntry in Handler.RowHandlers)
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
			List<AudioAssetRowControlHandler> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				AudioAssetRowControl arc = AssetEditor.Items.OfType<AudioAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].Asset);
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

			AudioAssetRowControl arc = e.AddedItems[0] as AudioAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_nameSort);

		private void TagsSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_tagsSort);

		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_descriptionSort);

		private void LoudnessSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_loudnessSort);

		private void PathSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_pathSort);

		private void SetSorting(AssetRowSorting<AudioAssetRowControlHandler> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> Handler?.UpdateTagHighlighting();
	}
}