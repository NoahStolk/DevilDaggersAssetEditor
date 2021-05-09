﻿using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.Binaries.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
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
		private readonly List<LocalFile> _localFiles = new();
		private readonly List<EffectiveChunk> _effectiveChunks = new();

		private string? _selectedPath;

		public ManageModsControl()
		{
			InitializeComponent();

			PopulateModFilesList();
		}

		private void PopulateModFilesList()
		{
			_localFiles.Clear();
			_effectiveChunks.Clear();

			ModFilesListView.Items.Clear();
			EffectiveChunkListView.Children.Clear();

			string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");
			ModsDirectoryLabel.Text = $"Files in mods directory ({modsDirectory})";
			foreach (string filePath in Directory.GetFiles(modsDirectory).OrderBy(p => Path.GetFileName(p).TrimStart('_')))
			{
				string fileName = Path.GetFileName(filePath);
				bool isValidFile = BinaryHandler.IsValidFile(filePath);
				bool isActiveFile = isValidFile && (fileName.StartsWith("audio") || fileName.StartsWith("dd"));

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
							EffectiveChunk? existingEffectiveChunk = _effectiveChunks.Find(ec => ec.AssetType == chunk.AssetType && ec.AssetName == chunk.Name);
							if (existingEffectiveChunk == null)
							{
								bool? isProhibited = AssetContainer.Instance.IsProhibited(chunk.Name, chunk.AssetType);
								_effectiveChunks.Add(new EffectiveChunk(fileName, chunk.AssetType, chunk.Name, isProhibited));
							}
							else
							{
								_effectiveChunks.Remove(existingEffectiveChunk);
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
								_effectiveChunks.Add(existingEffectiveChunk with { BinaryName = fileName });
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
							}
						}
					}
				}

				_localFiles.Add(new(filePath, chunks));

				Grid grid = new() { Height = 24 };
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(4, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new(2, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				TextBlock textBlock = new()
				{
					Text = fileName,
					IsEnabled = isValidFile,
					Foreground = ColorUtils.ThemeColors[isActiveFile ? "SuccessText" : isValidFile ? "Text" : "Gray6"],
					FontSize = 16,
				};
				grid.Children.Add(textBlock);

				if (isValidFile)
				{
					Button buttonToggle = new() { Content = isActiveFile ? "Disable binary" : "Enable binary" };
					buttonToggle.Click += (_, _) =>
					{
						string dir = Path.GetDirectoryName(filePath)!;
						if (isActiveFile)
							File.Move(filePath, Path.Combine(dir, $"_{fileName}"));
						else
							File.Move(filePath, Path.Combine(dir, fileName.TrimStart('_')));

						PopulateModFilesList();
					};
					Grid.SetColumn(buttonToggle, 1);
					grid.Children.Add(buttonToggle);

					Button buttonToggleProhibited = new() { Content = hasProhibitedAssets ? "Disable prohibited assets" : "Enable prohibited assets" };
					Grid.SetColumn(buttonToggleProhibited, 2);
					grid.Children.Add(buttonToggleProhibited);

					Button buttonDelete = new() { Content = "Delete file" };
					buttonDelete.Click += (_, _) =>
					{
						ConfirmWindow confirmWindow = new("Delete file", $"Are you sure you want to delete the file '{fileName}'?", false);
						confirmWindow.ShowDialog();

						if (confirmWindow.IsConfirmed != true)
							return;

						File.Delete(filePath);
						PopulateModFilesList();
					};
					Grid.SetColumn(buttonDelete, 3);
					grid.Children.Add(buttonDelete);
				}

				ModFilesListView.Items.Add(grid);
			}

			foreach (EffectiveChunk ec in _effectiveChunks)
			{
				Grid effectiveChunkGrid = new();
				effectiveChunkGrid.ColumnDefinitions.Add(new());
				effectiveChunkGrid.ColumnDefinitions.Add(new() { Width = new(2, GridUnitType.Star) });
				effectiveChunkGrid.ColumnDefinitions.Add(new() { Width = new(2, GridUnitType.Star) });

				TextBlock textBlockType = new()
				{
					Text = ec.AssetType.ToString(),
					Background = new SolidColorBrush(EditorUtils.FromRgbTuple(ec.AssetType.GetColor()) * 0.25f),
				};
				effectiveChunkGrid.Children.Add(textBlockType);

				TextBlock textBlockBinary = new() { Text = ec.BinaryName };
				Grid.SetColumn(textBlockBinary, 1);
				effectiveChunkGrid.Children.Add(textBlockBinary);

				TextBlock textBlockName = new()
				{
					Text = ec.AssetName,
					Foreground = ColorUtils.ThemeColors[ec.IsProhibited.HasValue ? ec.IsProhibited.Value ? "ErrorText" : "Text" : "Gray6"],
				};
				Grid.SetColumn(textBlockName, 2);
				effectiveChunkGrid.Children.Add(textBlockName);

				EffectiveChunkListView.Children.Add(effectiveChunkGrid);
			}
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

			foreach (Chunk chunk in localFile.Chunks)
			{
				if (chunk.Name == "loudness")
					continue;

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

		private record LocalFile(string? FilePath, List<Chunk>? Chunks);

		private record EffectiveChunk(string BinaryName, AssetType AssetType, string AssetName, bool? IsProhibited);
	}
}
