using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Progress;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class TrimBinaryWindow : Window
	{
		private string? _originalFilePath;
		private string? _compareFilePath;
		private string? _outputFilePath;

		public TrimBinaryWindow()
		{
			InitializeComponent();

			Progress = new(
				new(value => App.Instance.Dispatcher.Invoke(() => ProgressDescription.Text = value)),
				new(value => App.Instance.Dispatcher.Invoke(() => ProgressBar.Value = value)));
		}

		public ProgressWrapper Progress { get; }

		private void BrowseOriginalButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new();
			fileDialog.OpenDevilDaggersModsFolder();

			if (fileDialog.ShowDialog() == true)
			{
				_originalFilePath = fileDialog.FileName;
				TextBoxOriginal.Text = _originalFilePath;
				UpdateButtonTrimBinary();
			}
		}

		private void TextBoxOriginal_TextChanged(object sender, TextChangedEventArgs e)
		{
			_originalFilePath = TextBoxOriginal.Text;
			UpdateButtonTrimBinary();
		}

		private void BrowseCompareButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new();
			fileDialog.OpenDevilDaggersModsFolder();

			if (fileDialog.ShowDialog() == true)
			{
				_compareFilePath = fileDialog.FileName;
				TextBoxCompare.Text = _compareFilePath;
				UpdateButtonTrimBinary();
			}
		}

		private void TextBoxCompare_TextChanged(object sender, TextChangedEventArgs e)
		{
			_compareFilePath = TextBoxCompare.Text;
			UpdateButtonTrimBinary();
		}

		private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog fileDialog = new();
			fileDialog.OpenDevilDaggersModsFolder();

			if (fileDialog.ShowDialog() == true)
			{
				_outputFilePath = fileDialog.FileName;
				TextBoxOutput.Text = _outputFilePath;
				UpdateButtonTrimBinary();
			}
		}

		private void TextBoxOutput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_outputFilePath = TextBoxOutput.Text;
			UpdateButtonTrimBinary();
		}

		public void UpdateButtonTrimBinary()
		{
			ButtonTrimBinary.IsEnabled = !string.IsNullOrWhiteSpace(_originalFilePath) && !string.IsNullOrWhiteSpace(_compareFilePath) && !string.IsNullOrWhiteSpace(_outputFilePath) && File.Exists(_originalFilePath) && File.Exists(_compareFilePath);
		}

		private async void TrimBinary_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_outputFilePath) || !ValidateFileExists(_originalFilePath) || !ValidateFileExists(_compareFilePath))
				return;

			List<Chunk> originalChunks = GetChunksFromFile(_originalFilePath!);
			List<Chunk> compareChunks = GetChunksFromFile(_compareFilePath!);
			if (originalChunks.Count == 0 || compareChunks.Count == 0)
				return;

			TrimLog.Children.Clear();

			await Task.Run(() =>
			{
				try
				{
					List<Chunk> remainingChunks = new();
					int i = 0;
					foreach (Chunk chunk in originalChunks)
					{
						App.Instance.Dispatcher.Invoke(() => Progress.Report($"Comparing {chunk.AssetType} chunk '{chunk.Name}'.", i++ / (float)originalChunks.Count * 0.5f));

						Chunk? compareChunk = compareChunks.Find(c => c.AssetType == chunk.AssetType && c.Name == chunk.Name);
						if (chunk.IsBinaryEqual(compareChunk, out string? diffReason))
						{
							App.Instance.Dispatcher.Invoke(() => TrimLog.Children.Add(new TextBlock { Text = $"{chunk.AssetType} chunk '{chunk.Name}' will be removed because the chunks are identical.", Foreground = ColorUtils.ThemeColors["ErrorText"] }));
						}
						else
						{
							App.Instance.Dispatcher.Invoke(() => TrimLog.Children.Add(new TextBlock { Text = $"{chunk.AssetType} chunk '{chunk.Name}' will be kept: {diffReason}", Foreground = ColorUtils.ThemeColors["SuccessText"] }));
							remainingChunks.Add(chunk);
						}

						App.Instance.Dispatcher.Invoke(() => TrimScrollViewer.ScrollToEnd());
					}

					App.Instance.Dispatcher.Invoke(() => Progress.Report("Creating TOC stream.", 0.5f));
					BinaryFileHandler.CreateTocStream(remainingChunks, out byte[] tocBuffer, out Dictionary<Chunk, long> startOffsetBytePositions);
					byte[] assetBuffer = BinaryFileHandler.CreateAssetStream(remainingChunks, tocBuffer, startOffsetBytePositions, Progress);

					App.Instance.Dispatcher.Invoke(() => Progress.Report("Creating file.", 1));
					byte[] binaryBytes = BinaryFileHandler.CreateBinary(tocBuffer, assetBuffer);

					App.Instance.Dispatcher.Invoke(() => Progress.Report("Writing file.", 1));
					File.WriteAllBytes(_outputFilePath, binaryBytes);

					App.Instance.Dispatcher.Invoke(() => Progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Comparing binaries did not complete successfully", "An error occurred while comparing binaries.", ex);
						Progress.Report("Execution did not complete successfully.");
					});
				}
			});

			static bool ValidateFileExists(string? filePath)
			{
				if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
				{
					MessageWindow window = new("File not found.", $"The file at {filePath} was not found.");
					window.ShowDialog();
					return false;
				}

				return true;
			}

			static List<Chunk> GetChunksFromFile(string filePath)
			{
				byte[] fileBytes = File.ReadAllBytes(filePath);
				if (!BinaryFileHandler.IsValidFile(fileBytes))
				{
					MessageWindow window = new("Invalid file format.", "Make sure to open one of the following binary files: audio, core, dd");
					window.ShowDialog();
					return new();
				}

				byte[] tocBuffer = BinaryFileHandler.ReadTocBuffer(fileBytes);
				List<Chunk> chunks = BinaryFileHandler.ReadChunks(tocBuffer);

				foreach (Chunk chunk in chunks)
				{
					if (chunk.Size == 0) // Filter empty chunks (garbage in TOC buffers).
						continue;

					chunk.Buffer = new byte[chunk.Size];
					Buffer.BlockCopy(fileBytes, (int)chunk.StartOffset, chunk.Buffer, 0, (int)chunk.Size);
				}

				return chunks;
			}
		}
	}
}
