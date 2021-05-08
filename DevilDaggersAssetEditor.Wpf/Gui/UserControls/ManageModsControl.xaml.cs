using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.User;
using DevilDaggersCore.Wpf.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class ManageModsControl : UserControl
	{
		private readonly List<LocalFile> _localFiles = new();

		private string? _selectedPath;

		public ManageModsControl()
		{
			InitializeComponent();

			PopulateModFilesList();
		}

		private void PopulateModFilesList()
		{
			_localFiles.Clear();

			string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");
			ModsDirectoryLabel.Content = $"Files in mods directory ({modsDirectory})";
			foreach (string filePath in Directory.GetFiles(modsDirectory).OrderBy(p => p))
			{
				string fileName = Path.GetFileName(filePath);
				bool isValidFile = BinaryFileHandler.IsValidFile(filePath);
				bool isActiveFile = isValidFile && (fileName.StartsWith("audio") || fileName.StartsWith("dd"));

				TextBlock textBlock = new()
				{
					Text = fileName,
					IsEnabled = isValidFile,
					Foreground = ColorUtils.ThemeColors[isActiveFile ? "SuccessText" : isValidFile ? "Text" : "Gray6"],
				};
				_localFiles.Add(new(textBlock, filePath, isValidFile));

				ModFilesListView.Items.Add(textBlock);
			}
		}

		private void ModFilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LocalFile localFile = _localFiles[ModFilesListView.SelectedIndex];
			_selectedPath = localFile.FilePath;
			ChunkListView.Children.Clear();

			if (string.IsNullOrWhiteSpace(_selectedPath) || !File.Exists(_selectedPath) || !localFile.IsValidFile)
				return;

			byte[] tocBuffer = BinaryFileHandler.ReadTocBuffer(_selectedPath);
			foreach (Chunk chunk in BinaryFileHandler.ReadChunks(tocBuffer))
			{
				if (chunk.Name == "loudness")
					continue;

				bool? isProhibited = AssetContainer.Instance.IsProhibited(chunk.Name, chunk.AssetType);

				ChunkListView.Children.Add(new TextBlock
				{
					Text = chunk.Name,
					Foreground = ColorUtils.ThemeColors[isProhibited.HasValue ? isProhibited.Value ? "ErrorText" : "Text" : "Gray6"],
				});
			}
		}

		private class LocalFile
		{
			public LocalFile(TextBlock textBlock, string? filePath, bool isValidFile)
			{
				TextBlock = textBlock;
				FilePath = filePath;
				IsValidFile = isValidFile;
			}

			public TextBlock TextBlock { get; }
			public string? FilePath { get; }
			public bool IsValidFile { get; }
		}
	}
}
