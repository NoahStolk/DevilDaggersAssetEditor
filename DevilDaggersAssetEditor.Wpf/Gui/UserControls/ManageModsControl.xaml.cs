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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class ManageModsControl : UserControl
	{
		private static readonly SolidColorBrush _transparentBrush = new(Color.FromArgb(0, 0, 0, 0));
		private static readonly SolidColorBrush _highlightBrush = new(Color.FromArgb(63, 0, 255, 0));

		private List<LocalFile> _localFiles = new();
		private readonly List<EffectiveChunk> _effectiveChunks = new();

		private readonly Dictionary<EffectiveChunk, TextBlock> _effectiveChunkUi = new();

		private string? _selectedPath;

		private int _modFileListViewSelectedIndex = -1;

		public ManageModsControl()
		{
			InitializeComponent();

			PopulateModFilesList();
			PopulateEffectiveChunks();
		}

		#region UI creation

		public void PopulateModFilesList()
		{
			ModFilesListView.Items.Clear();

			string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");
			if (!Directory.Exists(modsDirectory))
			{
				MessageWindow window = new("Mods directory not found", $"The directory '{modsDirectory}' does not exist. Please make sure that the Devil Daggers root folder in the Settings window is correct, and that a 'mods' folder is present.");
				window.ShowDialog();
				return;
			}

			ModsDirectoryLabel.Text = $"Files in mods directory ({modsDirectory})";

			// Populate mod file listing.
			foreach (string filePath in Directory.GetFiles(modsDirectory).OrderBy(p => Path.GetFileName(p).TrimStart('_')))
			{
				LocalFile localFile = GetOrCreateLocalFile(filePath);

				// Populate mod file listing UI.
				Grid grid = new() { Height = 24 };
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(4, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new());
				grid.ColumnDefinitions.Add(new());

				TextBlock textBlock = new()
				{
					Text = localFile.FileName,
					IsEnabled = localFile.IsValidFile,
					Foreground = ColorUtils.ThemeColors[GetColor(localFile.HasValidName, localFile.IsActiveFile, localFile.IsValidFile)],
					FontSize = 16,
				};
				grid.Children.Add(textBlock);

				Button buttonRename = new() { Content = "Rename file" };
				buttonRename.Click += (_, _) => RenameFile(filePath, localFile.FileName);
				Grid.SetColumn(buttonRename, 1);
				grid.Children.Add(buttonRename);

				Button buttonDelete = new() { Content = "Delete file" };
				buttonDelete.Click += (_, _) => DeleteFile(filePath, localFile.FileName);
				Grid.SetColumn(buttonDelete, 2);
				grid.Children.Add(buttonDelete);

				Button buttonToggle = new()
				{
					Content = localFile.IsActiveFile ? "Disable file" : "Enable file",
					IsEnabled = localFile.IsValidFile && localFile.HasValidName,
					Foreground = localFile.IsValidFile && localFile.HasValidName && !localFile.IsActiveFile ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["Text"],
				};
				buttonToggle.Click += (_, _) => ToggleFile(filePath, localFile.FileName, localFile.IsActiveFile);
				Grid.SetColumn(buttonToggle, 3);
				grid.Children.Add(buttonToggle);

				Button buttonToggleProhibited = new()
				{
					Content = localFile.AreProhibitedAssetsEnabled ? "Disable prohibited" : "Enable prohibited",
					IsEnabled = localFile.IsValidFile && localFile.HasProhibitedAssets,
					FontSize = 9,
					Foreground = localFile.AreProhibitedAssetsEnabled ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["WarningText"],
				};
				buttonToggleProhibited.Click += (_, _) => ToggleProhibited(filePath, localFile.HasProhibitedAssets, localFile.AreProhibitedAssetsEnabled);
				Grid.SetColumn(buttonToggleProhibited, 4);
				grid.Children.Add(buttonToggleProhibited);

				ModFilesListView.Items.Add(grid);
			}
		}

		private void PopulateEffectiveChunks()
		{
			_effectiveChunks.Clear();
			foreach (LocalFile localFile in _localFiles.OrderBy(lf => lf.FileName))
			{
				if (!localFile.IsActiveFile || localFile.Chunks == null)
					continue;

				foreach (Chunk chunk in localFile.Chunks)
				{
					if (chunk.AssetType == AssetType.Audio && chunk.Name == "loudness")
						continue;

					EffectiveChunk? existingEffectiveChunk = _effectiveChunks.Find(ec => ec.AssetType == chunk.AssetType && ec.AssetName == chunk.Name);
					if (existingEffectiveChunk == null)
						_effectiveChunks.Add(new EffectiveChunk(localFile.FileName, chunk.AssetType, chunk.Name, AssetContainer.Instance.IsProhibited(chunk.Name, chunk.AssetType)));
					else
						existingEffectiveChunk.BinaryName = localFile.FileName;
				}
			}

			_effectiveChunkUi.Clear();
			EffectiveChunkListView.Children.Clear();
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
						Foreground = ColorUtils.ThemeColors[ec.IsProhibited.HasValue ? ec.IsProhibited.Value ? "WarningText" : "Text" : "Gray6"],
					};
					Grid.SetColumn(textBlockName, 1);
					effectiveChunkGrid.Children.Add(textBlockName);

					EffectiveChunkListView.Children.Add(effectiveChunkGrid);

					_effectiveChunkUi.Add(ec, textBlockName);
				}
			}
		}

		#endregion UI creation

		#region Local files

		private void SortLocalFiles()
			=> _localFiles = _localFiles.OrderBy(lf => lf.FileName.TrimStart('_')).ToList();

		private LocalFile? GetLocalFile(string filePath)
			=> _localFiles.Find(lf => lf.FilePath == filePath);

		private LocalFile GetOrCreateLocalFile(string filePath)
		{
			LocalFile? localFile = GetLocalFile(filePath);
			if (localFile != null)
				return localFile;

			localFile = new(filePath);
			_localFiles.Add(localFile);
			return localFile;
		}

		#endregion Local files

		#region Actions

		private void RenameFile(string filePath, string fileName)
		{
			if (!File.Exists(filePath))
				return;

			string? directory = Path.GetDirectoryName(filePath);
			if (string.IsNullOrWhiteSpace(directory))
				return;

			RenameFileWindow renameFileWindow = new(directory, fileName);
			renameFileWindow.ShowDialog();

			if (string.IsNullOrEmpty(renameFileWindow.NewFileName))
				return;

			string newFilePath = Path.Combine(directory, renameFileWindow.NewFileName);
			File.Move(filePath, newFilePath);

			GetLocalFile(filePath)?.UpdateFilePathProperties(newFilePath);
			SortLocalFiles();

			PopulateModFilesList();
			PopulateEffectiveChunks();

			ModFilesListView.SelectedIndex = _modFileListViewSelectedIndex;
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

			LocalFile? localFile = GetLocalFile(filePath);
			if (localFile != null)
				_localFiles.Remove(localFile);

			PopulateModFilesList();
			PopulateEffectiveChunks();
		}

		private void ToggleFile(string filePath, string fileName, bool isActiveFile)
		{
			if (!File.Exists(filePath))
				return;

			string directory = Path.GetDirectoryName(filePath)!;
			string newFileName = isActiveFile ? $"_{fileName}" : fileName.TrimStart('_');
			string newFilePath = Path.Combine(directory, newFileName);
			File.Move(filePath, newFilePath);

			GetLocalFile(filePath)?.UpdateFilePathProperties(newFilePath);
			SortLocalFiles();

			PopulateModFilesList();
			PopulateEffectiveChunks();

			ModFilesListView.SelectedIndex = _modFileListViewSelectedIndex;
		}

		private void ToggleProhibited(string filePath, bool hasProhibitedAssets, bool areProhibitedAssetsEnabled)
		{
			if (!File.Exists(filePath) || !BinaryHandler.IsValidFile(filePath) || !hasProhibitedAssets)
				return;

			byte[] tocBuffer = BinaryHandler.ReadTocBuffer(filePath);
			using (FileStream fs = new(filePath, FileMode.Open))
			{
				fs.Seek(12, SeekOrigin.Begin);

				foreach (Chunk chunk in BinaryHandler.ReadChunks(tocBuffer))
				{
					if (AssetContainer.Instance.IsProhibited(chunk.Name.ToLower(), chunk.AssetType) == true)
					{
						if (areProhibitedAssetsEnabled)
							chunk.Disable();
						else
							chunk.Enable();
					}

					fs.Position += 2; // Type byte, empty byte
					fs.Write(Encoding.Default.GetBytes(chunk.Name), 0, chunk.Name.Length);
					fs.Position += 1 + sizeof(uint) * 3; // Null terminator, start offset, size, unknown
				}
			}

			GetLocalFile(filePath)?.UpdateFileContentProperties();

			PopulateModFilesList();
			PopulateEffectiveChunks();

			ModFilesListView.SelectedIndex = _modFileListViewSelectedIndex;
		}

		#endregion Actions

		#region Events

		private void ModFilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ChunkListView.Children.Clear();

			if (ModFilesListView.SelectedIndex == -1)
				return;

			_modFileListViewSelectedIndex = ModFilesListView.SelectedIndex;

			ChunkListScrollViewer.ScrollToTop();
			EffectiveChunkListScrollViewer.ScrollToTop();

			LocalFile localFile = _localFiles[ModFilesListView.SelectedIndex];
			_selectedPath = localFile.FilePath;

			if (string.IsNullOrWhiteSpace(_selectedPath) || !File.Exists(_selectedPath) || localFile.Chunks == null)
				return;

			string? binaryName = Path.GetFileName(localFile.FilePath);
			TextBlock textBlockBinary = new()
			{
				Text = binaryName,
				Background = ColorUtils.ThemeColors["Gray4"],
				FontSize = 14,
				Padding = new(0, 4, 0, 0),
				FontWeight = FontWeights.Bold,
			};
			ChunkListView.Children.Add(textBlockBinary);

			// Clear effective chunks highlight.
			foreach (KeyValuePair<EffectiveChunk, TextBlock> kvp in _effectiveChunkUi)
				kvp.Value.Background = _transparentBrush;

			foreach (Chunk chunk in localFile.Chunks)
			{
				if (chunk.AssetType == AssetType.Audio && chunk.Name == "loudness")
					continue;

				// Highlight effective chunk.
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
					Foreground = ColorUtils.ThemeColors[isProhibited.HasValue ? isProhibited.Value ? "WarningText" : "Text" : "Gray6"],
				};
				Grid.SetColumn(textBlockName, 1);
				grid.Children.Add(textBlockName);

				ChunkListView.Children.Add(grid);
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			PopulateModFilesList();
			PopulateEffectiveChunks();
		}

		#endregion Events

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

		private class LocalFile
		{
			public LocalFile(string filePath)
			{
				UpdateFilePathProperties(filePath);
				UpdateFileContentProperties();
			}

			public string FilePath { get; set; } = null!; // Set in UpdateFilePathProperties method.
			public string FileName { get; set; } = null!; // Set in UpdateFilePathProperties method.
			public bool IsValidFile { get; set; }
			public bool IsActiveFile { get; set; }
			public bool HasValidName { get; set; }

			public List<Chunk>? Chunks { get; set; }
			public bool HasProhibitedAssets { get; set; }
			public bool AreProhibitedAssetsEnabled { get; set; }

			public void UpdateFilePathProperties(string filePath)
			{
				FilePath = filePath;
				FileName = Path.GetFileName(filePath);
				IsValidFile = BinaryHandler.IsValidFile(filePath);
				IsActiveFile = IsValidFile && (FileName.StartsWith("audio") || FileName.StartsWith("dd"));
				HasValidName = FileName.StartsWith("audio") || FileName.StartsWith("dd") || FileName.StartsWith("_audio") || FileName.StartsWith("_dd");
			}

			public void UpdateFileContentProperties()
			{
				if (IsValidFile)
				{
					byte[] tocBuffer = BinaryHandler.ReadTocBuffer(FilePath);
					Chunks = BinaryHandler.ReadChunks(tocBuffer);
					HasProhibitedAssets = Chunks.Any(c => AssetContainer.Instance.IsProhibited(c.Name.ToLower(), c.AssetType) == true);
					AreProhibitedAssetsEnabled = Chunks.Any(c => AssetContainer.Instance.IsProhibited(c.Name, c.AssetType) == true);
				}
			}
		}

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
