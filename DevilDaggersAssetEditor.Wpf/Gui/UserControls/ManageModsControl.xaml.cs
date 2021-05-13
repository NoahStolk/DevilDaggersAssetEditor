using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.Binaries.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersCore.Wpf.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class ManageModsControl : UserControl
	{
		private static readonly SolidColorBrush _transparentBrush = new(Color.FromArgb(0, 0, 0, 0));
		private static readonly SolidColorBrush _highlightBrush = new(Color.FromArgb(63, 0, 255, 0));

		private readonly List<LocalFile> _localFiles = new();
		private readonly List<EffectiveChunk> _effectiveChunks = new();

		private readonly Dictionary<EffectiveChunk, TextBlock> _effectiveChunkUi = new();

		private string? _selectedPath;

		public ManageModsControl()
		{
			InitializeComponent();

			PopulateModFilesList();
		}

		public void PopulateModFilesList()
		{
			_localFiles.Clear();
			_effectiveChunks.Clear();
			_effectiveChunkUi.Clear();

			ModFilesListView.Items.Clear();
			EffectiveChunkListView.Children.Clear();

			string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");
			ModsDirectoryLabel.Text = $"Files in mods directory ({modsDirectory})";

			// Populate mod file listing.
			foreach (string filePath in Directory.GetFiles(modsDirectory).OrderBy(p => Path.GetFileName(p).TrimStart('_')))
			{
				// Determine file validity.
				string fileName = Path.GetFileName(filePath);
				bool isValidFile = BinaryHandler.IsValidFile(filePath);
				bool isActiveFile = isValidFile && (fileName.StartsWith("audio") || fileName.StartsWith("dd"));
				bool hasValidName = fileName.StartsWith("audio") || fileName.StartsWith("dd") || fileName.StartsWith("_audio") || fileName.StartsWith("_dd");

				// Determine prohibited assets and effective chunks.
				List<Chunk>? chunks = null;
				bool hasProhibitedAssets = false;
				if (isValidFile)
				{
					byte[] tocBuffer = BinaryHandler.ReadTocBuffer(filePath);
					chunks = BinaryHandler.ReadChunks(tocBuffer);
					hasProhibitedAssets = chunks.Any(c => AssetContainer.Instance.IsProhibited(c.Name, c.AssetType) == true);

					if (isActiveFile)
					{
						foreach (Chunk chunk in chunks)
						{
							if (chunk.AssetType == AssetType.Audio && chunk.Name == "loudness")
								continue;

							EffectiveChunk? existingEffectiveChunk = _effectiveChunks.Find(ec => ec.AssetType == chunk.AssetType && ec.AssetName == chunk.Name);
							if (existingEffectiveChunk == null)
								_effectiveChunks.Add(new EffectiveChunk(fileName, chunk.AssetType, chunk.Name, AssetContainer.Instance.IsProhibited(chunk.Name, chunk.AssetType)));
							else
								existingEffectiveChunk.BinaryName = fileName;
						}
					}
				}

				_localFiles.Add(new(filePath, chunks));

				// Populate mod file listing UI.
				Grid grid = new() { Height = 24 };
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(4, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new());

				TextBlock textBlock = new()
				{
					Text = fileName,
					IsEnabled = isValidFile,
					Foreground = ColorUtils.ThemeColors[GetColor(hasValidName, isActiveFile, isValidFile)],
					FontSize = 16,
				};
				grid.Children.Add(textBlock);

				Button buttonRename = new() { Content = "Rename file" };
				buttonRename.Click += (_, _) => RenameFile(filePath, fileName);
				Grid.SetColumn(buttonRename, 1);
				grid.Children.Add(buttonRename);

				Button buttonDelete = new() { Content = "Delete file" };
				buttonDelete.Click += (_, _) => DeleteFile(filePath, fileName);
				Grid.SetColumn(buttonDelete, 2);
				grid.Children.Add(buttonDelete);

				Button buttonToggle = new()
				{
					Content = isActiveFile ? "Disable file" : "Enable file",
					IsEnabled = isValidFile && hasValidName,
					Foreground = isValidFile && hasValidName && !isActiveFile ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["Text"],
				};
				buttonToggle.Click += (_, _) => ToggleFile(filePath, fileName, isActiveFile);
				Grid.SetColumn(buttonToggle, 3);
				grid.Children.Add(buttonToggle);

				Button buttonToggleProhibited = new()
				{
					Content = hasProhibitedAssets ? "Disable prohibited" : "Enable prohibited",
					IsEnabled = isValidFile,
					FontSize = 9,
				};
				buttonToggleProhibited.Click += (_, _) => { };
				Grid.SetColumn(buttonToggleProhibited, 4);
				grid.Children.Add(buttonToggleProhibited);

				ModFilesListView.Items.Add(grid);
			}

			// Populate effective chunks UI.
			foreach (IGrouping<string, EffectiveChunk> ecg in _effectiveChunks.GroupBy(e => e.BinaryName))
			{
				TextBlock textBlockBinary = new()
				{
					Text = ecg.Key,
					Background = ColorUtils.ThemeColors["Gray4"],
					FontSize = 14,
					Padding = new(0, 4, 0, 0),
					FontWeight = FontWeights.Bold,
				};
				Grid.SetColumn(textBlockBinary, 1);
				EffectiveChunkListView.Children.Add(textBlockBinary);

				foreach (EffectiveChunk ec in ecg)
				{
					Grid effectiveChunkGrid = new();
					effectiveChunkGrid.ColumnDefinitions.Add(new());
					effectiveChunkGrid.ColumnDefinitions.Add(new() { Width = new(3, GridUnitType.Star) });

					TextBlock textBlockType = new()
					{
						Text = ec.AssetType.ToString(),
						Background = new SolidColorBrush(EditorUtils.FromRgbTuple(ec.AssetType.GetColor()) * 0.25f),
					};
					effectiveChunkGrid.Children.Add(textBlockType);

					TextBlock textBlockName = new()
					{
						Text = ec.AssetName,
						Foreground = ColorUtils.ThemeColors[ec.IsProhibited.HasValue ? ec.IsProhibited.Value ? "ErrorText" : "Text" : "Gray6"],
					};
					Grid.SetColumn(textBlockName, 1);
					effectiveChunkGrid.Children.Add(textBlockName);

					EffectiveChunkListView.Children.Add(effectiveChunkGrid);

					_effectiveChunkUi.Add(ec, textBlockName);
				}
			}
		}

		private void RenameFile(string filePath, string fileName)
		{
			if (!File.Exists(filePath))
				return;

			RenameFileWindow renameFileWindow = new(fileName);
			renameFileWindow.ShowDialog();

			if (string.IsNullOrEmpty(renameFileWindow.NewFileName))
				return;

			string dir = Path.GetDirectoryName(filePath)!;
			File.Move(filePath, Path.Combine(dir, renameFileWindow.NewFileName));
			PopulateModFilesList();
		}

		private void DeleteFile(string filePath, string fileName)
		{
			if (!File.Exists(filePath))
				return;

			ConfirmWindow confirmWindow = new("Delete file", $"Are you sure you want to delete the file '{fileName}'?", false);
			confirmWindow.ShowDialog();

			if (confirmWindow.IsConfirmed != true)
				return;

			File.Delete(filePath);
			PopulateModFilesList();
		}

		private void ToggleFile(string filePath, string fileName, bool isActiveFile)
		{
			if (!File.Exists(filePath))
				return;

			string dir = Path.GetDirectoryName(filePath)!;
			if (isActiveFile)
				File.Move(filePath, Path.Combine(dir, $"_{fileName}"));
			else
				File.Move(filePath, Path.Combine(dir, fileName.TrimStart('_')));

			PopulateModFilesList();
		}

		private void ModFilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ModFilesListView.SelectedIndex == -1)
				return;

			ChunkListScrollViewer.ScrollToTop();
			EffectiveChunkListScrollViewer.ScrollToTop();

			LocalFile localFile = _localFiles[ModFilesListView.SelectedIndex];
			_selectedPath = localFile.FilePath;
			ChunkListView.Children.Clear();

			if (string.IsNullOrWhiteSpace(_selectedPath) || !File.Exists(_selectedPath) || localFile.Chunks == null)
				return;

			// Clear effective chunks highlight.
			foreach (KeyValuePair<EffectiveChunk, TextBlock> kvp in _effectiveChunkUi)
				kvp.Value.Background = _transparentBrush;

			foreach (Chunk chunk in localFile.Chunks)
			{
				if (chunk.AssetType == AssetType.Audio && chunk.Name == "loudness")
					continue;

				// Highlight effective chunk.
				string? binaryName = Path.GetFileName(localFile.FilePath);
				EffectiveChunk? effectiveChunk = _effectiveChunks.Find(ec => ec.AssetName == chunk.Name && ec.AssetType == chunk.AssetType && ec.BinaryName == binaryName);
				if (effectiveChunk != null)
					_effectiveChunkUi[effectiveChunk].Background = _highlightBrush;

				// Populate binary contents UI.
				bool? isProhibited = AssetContainer.Instance.IsProhibited(chunk.Name, chunk.AssetType);

				Grid grid = new();
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new() { Width = new(3, GridUnitType.Star) });

				TextBlock textBlockType = new()
				{
					Text = chunk.AssetType.ToString(),
					Background = new SolidColorBrush(EditorUtils.FromRgbTuple(chunk.AssetType.GetColor()) * 0.25f),
				};
				grid.Children.Add(textBlockType);

				TextBlock textBlockName = new()
				{
					Text = chunk.Name,
					Foreground = ColorUtils.ThemeColors[isProhibited.HasValue ? isProhibited.Value ? "ErrorText" : "Text" : "Gray6"],
				};
				Grid.SetColumn(textBlockName, 1);
				grid.Children.Add(textBlockName);

				ChunkListView.Children.Add(grid);
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
			=> PopulateModFilesList();

		private static string GetColor(bool hasValidName, bool isActiveFile, bool isValidFile)
		{
			if (!isValidFile)
				return "Gray6";
			if (!hasValidName)
				return "ErrorText";
			if (!isActiveFile)
				return "Text";
			return "SuccessText";
		}

		private record LocalFile(string? FilePath, List<Chunk>? Chunks);

		private class EffectiveChunk
		{
			public EffectiveChunk(string binaryName, AssetType assetType, string assetName, bool? isProhibited)
			{
				BinaryName = binaryName;
				AssetType = assetType;
				AssetName = assetName;
				IsProhibited = isProhibited;
			}

			public string BinaryName { get; set; }
			public AssetType AssetType { get; set; }
			public string AssetName { get; set; }
			public bool? IsProhibited { get; set; }
		}
	}
}
