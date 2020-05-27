using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetTabControls
{
	public partial class AudioAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(AudioAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AudioAssetTabControlHandler Handler { get; private set; }

		private readonly AssetRowSorting<AudioAsset, AudioAssetRowControl> nameSort = new AssetRowSorting<AudioAsset, AudioAssetRowControl>((a) => a.Asset.AssetName);
		private readonly AssetRowSorting<AudioAsset, AudioAssetRowControl> tagsSort = new AssetRowSorting<AudioAsset, AudioAssetRowControl>((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting<AudioAsset, AudioAssetRowControl> descriptionSort = new AssetRowSorting<AudioAsset, AudioAssetRowControl>((a) => a.Asset.Description);
		private readonly AssetRowSorting<AudioAsset, AudioAssetRowControl> loudnessSort = new AssetRowSorting<AudioAsset, AudioAssetRowControl>((a) => a.Asset.Loudness);
		private readonly AssetRowSorting<AudioAsset, AudioAssetRowControl> pathSort = new AssetRowSorting<AudioAsset, AudioAssetRowControl>((a) => a.Asset.EditorPath);

		public AudioAssetTabControl()
		{
			InitializeComponent();

			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
			timer.Tick += (sender, e) =>
			{
				if (Previewer.Song == null || Previewer.Song.Paused)
					return;

				if (!Previewer.Dragging)
					Previewer.Seek.Value = Previewer.Song.PlayPosition / (float)Previewer.Song.PlayLength * Previewer.Seek.Maximum;

				Previewer.SeekText.Text = $"{EditorUtils.ToTimeString((int)Previewer.Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Previewer.Song.PlayLength)}";
			};
			timer.Start();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (AudioAssetRowControl arc in Handler.AssetRowEntries.Select(a => a.AssetRowControl))
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
				Handler.AssetRowEntries.Select(a => new KeyValuePair<AudioAssetRowControl, TextBlock>(a.AssetRowControl, a.AssetRowControl.Handler.TextBlockTags)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (AssetRowEntry<AudioAsset, AudioAssetRowControl> are in Handler.AssetRowEntries)
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
			List<AssetRowEntry<AudioAsset, AudioAssetRowControl>> sorted = Handler.ApplySort();
			for (int i = 0; i < sorted.Count; i++)
			{
				AudioAssetRowControl arc = AssetEditor.Items.OfType<AudioAssetRowControl>().FirstOrDefault(arc => arc.Handler.Asset == sorted[i].Asset);
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
			List<AudioAssetRowControl> rows = AssetEditor.Items.OfType<AudioAssetRowControl>().ToList();
			foreach (AudioAssetRowControl row in rows)
				row.Handler.UpdateBackgroundRectangleColors(rows.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			AudioAssetRowControl arc = e.AddedItems[0] as AudioAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(nameSort);
		private void TagsSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(tagsSort);
		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(descriptionSort);
		private void LoudnessSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(loudnessSort);
		private void PathSortButton_Click(object sender, RoutedEventArgs e) => SetSorting(pathSort);

		private void SetSorting(AssetRowSorting<AudioAsset, AudioAssetRowControl> sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			Handler.ActiveSorting = sorting;

			ApplySort();
		}
	}

	public class AudioAssetTabControlHandler : AbstractAssetTabControlHandler<AudioAsset, AudioAssetRowControl>
	{
		protected override string AssetTypeJsonFileName => "Audio";

		private UserSettings Settings => UserHandler.Instance.settings;

		public AudioAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(AudioAsset asset)
		{
			AudioAssetRowControl arc = AssetRowEntries.FirstOrDefault(a => a.Asset == asset).AssetRowControl;
			arc.TextBlockEditorPath.Text = asset.EditorPath;
			arc.TextBoxLoudness.Text = asset.Loudness.ToString();
			arc.Handler.UpdateGui();
		}

		public void ImportLoudness()
		{
			OpenFileDialog dialog = new OpenFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (Settings.EnableModsRootFolder)
				dialog.InitialDirectory = Settings.ModsRootFolder;
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Dictionary<string, float> values = new Dictionary<string, float>();
			int lineNumber = 0;
			foreach (string line in File.ReadAllLines(dialog.FileName))
			{
				lineNumber++;
				string lineClean = line
					.Replace(" ", "") // Remove spaces to make things easier.
					.TrimEnd('.'); // Remove dots at the end of the line. (The original loudness file has one on line 154 for some reason...)
				if (!LoudnessUtils.ReadLoudnessLine(lineClean, out string assetName, out float loudness))
				{
					App.Instance.ShowMessage($"Syntax error on line {lineNumber}", "Could not parse loudness file.");
					return;
				}

				values[assetName] = loudness;
			}

			int successCount = 0;
			int unchangedCount = 0;
			foreach (KeyValuePair<string, float> kvp in values)
			{
				AssetRowEntry<AudioAsset, AudioAssetRowControl> assetRowEntry = AssetRowEntries.FirstOrDefault(a => a.Asset.AssetName == kvp.Key);
				AudioAsset audioAsset = assetRowEntry.Asset;
				if (audioAsset != null)
				{
					if (audioAsset.Loudness == kvp.Value)
					{
						unchangedCount++;
					}
					else
					{
						audioAsset.Loudness = kvp.Value;
						successCount++;
					}

					AudioAssetRowControl arc = assetRowEntry.AssetRowControl;
					arc.Handler.UpdateGui();
				}
			}

			App.Instance.ShowMessage("Loudness import results", $"Total audio assets: {AssetRowEntries.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}");
		}

		public void ExportLoudness()
		{
			SaveFileDialog dialog = new SaveFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (Settings.EnableModsRootFolder)
				dialog.InitialDirectory = Settings.ModsRootFolder;
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			StringBuilder sb = new StringBuilder();
			foreach (AudioAsset audioAsset in AssetRowEntries.Select(a => a.Asset))
				sb.AppendLine($"{audioAsset.AssetName} = {audioAsset.Loudness}");
			File.WriteAllText(dialog.FileName, sb.ToString());
		}
	}
}