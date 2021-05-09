using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.Binaries.Analyzer;
using DevilDaggersAssetEditor.Binaries.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Wpf.Utils;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class BinaryFileAnalyzerWindow : Window
	{
		private readonly int _columnCount;

		private static readonly SolidColorBrush _black = new(Color.FromRgb(0, 0, 0));

		public BinaryFileAnalyzerWindow()
		{
			InitializeComponent();

			_columnCount = ChunkData.ColumnDefinitions.Count;
		}

		private void OpenFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new();
			openDialog.OpenDevilDaggersRootFolder();

			bool? openResult = openDialog.ShowDialog();
			if (openResult == true)
			{
				AnalyzerFileResult? result = TryReadResourceFile(openDialog.FileName);
				if (result == null)
					App.Instance.ShowMessage("Invalid file format", "Make sure to open one of the following binary files: audio, core, dd");
				else
					ShowFileResult(result);
			}
		}

		private void ShowFileResult(AnalyzerFileResult fileResult)
		{
			ContentStackPanel.Visibility = Visibility.Visible;

			Data.Children.Clear();
			ChunkData.Children.Clear();
			ChunkData.RowDefinitions.Clear();

			Dictionary<string, AnalyzerChunkGroup> chunkGroups = new()
			{
				{ "File header", ChunkResult(Color.FromRgb(255, 127, 127), fileResult.HeaderByteCount, new()) },
			};

			IEnumerable<IGrouping<AssetType, Chunk>> chunksByType = fileResult.Chunks.GroupBy(c => c.AssetType);
			foreach (IGrouping<AssetType, Chunk> group in chunksByType.OrderBy(c => c.Key))
			{
				IEnumerable<Chunk> validChunks = group;
				int size = 0;
				int headerSize = 0;
				foreach (Chunk chunk in validChunks)
				{
					headerSize += chunk.HeaderSize;
					size += (int)chunk.Size - chunk.HeaderSize;
				}

				chunkGroups.Add(group.Key.ToString(), ChunkResult(GetColor(group.Key, false), size, validChunks.ToList()));
				if (headerSize > 0)
					chunkGroups.Add($"{group.Key} header", ChunkResult(GetColor(group.Key, true), headerSize, validChunks.ToList()));
			}

			int unknownSize = fileResult.FileByteCount - chunkGroups.Sum(c => c.Value.ByteCount);
			if (unknownSize > 0)
				chunkGroups.Add("Unknown", ChunkResult(Color.FromRgb(127, 127, 255), unknownSize, new()));

			const int totalHeight = 32;
			double pos = 0;

			FileName.Content = fileResult.FileName;
			FileSize.Content = $"{fileResult.FileByteCount:N0} bytes";
			float maxPercentage = chunkGroups.Max(c => c.Value.ByteCount / (float)fileResult.FileByteCount);
			int i = 0;
			foreach (KeyValuePair<string, AnalyzerChunkGroup> kvp in chunkGroups.OrderBy(c => c.Value.Chunks.Count != 0).ThenByDescending(c => c.Value.ByteCount).Where(c => c.Value.ByteCount > 0))
			{
				Color chunkColor = ChunkResultColor(kvp.Value);
				SolidColorBrush chunkBrush = new(chunkColor);

				double sizePercentage = kvp.Value.ByteCount / (double)fileResult.FileByteCount;
				double width = sizePercentage * Canvas.Width;
				Rectangle rect = new()
				{
					Width = width,
					Height = totalHeight,
					Fill = chunkBrush,
				};
				Canvas.SetLeft(rect, pos);
				Canvas.Children.Add(rect);

				pos += width;

				Rectangle rectColor = new() { Fill = chunkBrush };
				rectColor.Stroke = _black;
				rectColor.StrokeThickness = 1;
				rectColor.SnapsToDevicePixels = true;

				SolidColorBrush bgColor = ColorUtils.ThemeColors[i++ % 2 == 0 ? "Gray4" : "Gray5"];

				Rectangle rectBackground = new() { Fill = bgColor };
				rectBackground.Stroke = _black;
				rectBackground.StrokeThickness = 1;
				rectBackground.StrokeDashArray = new DoubleCollection(new List<double> { 1, 2 });
				rectBackground.SnapsToDevicePixels = true;
				rectBackground.Fill = new SolidColorBrush(ColorWithAlpha(chunkColor, 95));
				Brush textColor = GetTextColorBasedOnBackgroundColor(chunkColor);
				Label labelPercentage = new() { Content = sizePercentage.ToString("0.000%"), Foreground = textColor };

				Grid.SetColumn(rectColor, 0);
				Grid.SetColumn(rectBackground, 1);
				Grid.SetColumnSpan(labelPercentage, 2);

				Grid grid = new();
				double col0w = sizePercentage / maxPercentage * 100;
				double col1w = 100 - sizePercentage / maxPercentage * 100;
				if (col0w < 0.001)
					col0w = 0;
				if (col1w < 0.001)
					col1w = 0;
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col0w, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col1w, GridUnitType.Star) });
				grid.Children.Add(rectColor);
				grid.Children.Add(rectBackground);
				grid.Children.Add(labelPercentage);

				StackPanel stackPanel = new() { Background = bgColor };
				stackPanel.Children.Add(new Label { Content = $"{kvp.Key} data", FontWeight = FontWeights.Bold });
				stackPanel.Children.Add(grid);
				stackPanel.Children.Add(new Label { Content = $"{kvp.Value.ByteCount:N0} bytes" });
				if (kvp.Value.Chunks.Count > 0)
					stackPanel.Children.Add(new Label { Content = $"{kvp.Value.Chunks.Count} chunks" });

				Data.Children.Add(stackPanel);
			}

			int k = 0;
			foreach (Chunk chunk in fileResult.Chunks.OrderByDescending(c => c.Size))
			{
				if (k % _columnCount == 0)
					ChunkData.RowDefinitions.Add(new RowDefinition());

				double chunkSizePercentage = chunk.Size / (double)fileResult.FileByteCount;

				Color baseBgColor = GetColor(chunk.AssetType, false);
				Color bgColor = baseBgColor * (float)chunkSizePercentage * 50;
				Brush textColor = GetTextColorBasedOnBackgroundColor(bgColor);

				StackPanel stackPanel = new() { Background = new SolidColorBrush(bgColor) };
				TextBlock textBlockDataName = new() { Margin = new Thickness(2), Text = chunk.AssetType.ToString(), FontWeight = FontWeights.Bold, Foreground = textColor };
				if (chunkSizePercentage < 0.005f)
					textBlockDataName.Background = new SolidColorBrush(baseBgColor * 0.25f);
				stackPanel.Children.Add(textBlockDataName);
				stackPanel.Children.Add(new Label { Content = chunk.Name, FontWeight = FontWeights.Bold, Foreground = textColor });
				stackPanel.Children.Add(new Label { Content = $"{chunk.Size:N0} bytes", Foreground = textColor });
				stackPanel.Children.Add(new Label { Content = $"{chunkSizePercentage:0.000%} of file", Foreground = textColor });

				Border border = new() { BorderThickness = new Thickness(1), BorderBrush = _black };
				border.Child = stackPanel;
				Grid.SetColumn(border, k % _columnCount);
				Grid.SetRow(border, k / _columnCount);

				ChunkData.Children.Add(border);
				k++;
			}
		}

		private static Brush GetTextColorBasedOnBackgroundColor(Color backgroundColor)
			=> ColorUtils.GetPerceivedBrightness(backgroundColor) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];

		private static AnalyzerChunkGroup ChunkResult(Color color, int byteCount, List<Chunk> chunks)
			=> new(color.R, color.G, color.B, byteCount, chunks);

		private static Color ChunkResultColor(AnalyzerChunkGroup chunkResult)
			=> Color.FromRgb(chunkResult.R, chunkResult.G, chunkResult.B);

		private static Color ColorWithAlpha(Color color, byte alpha)
			=> Color.FromArgb(alpha, color.R, color.G, color.B);

		private static AnalyzerFileResult? TryReadResourceFile(string sourceFileName)
		{
			if (!BinaryHandler.IsValidFile(sourceFileName))
				return null;

			int length = (int)new FileInfo(sourceFileName).Length;

			byte[] tocBuffer = BinaryHandler.ReadTocBuffer(sourceFileName);
			return new(sourceFileName, length, tocBuffer.Length + BinaryHandler.HeaderSize, BinaryHandler.ReadChunks(tocBuffer));
		}

		private static Color GetColor(AssetType? assetType, bool isHeader)
		{
			if (assetType.HasValue)
				return EditorUtils.FromRgbTuple(assetType.Value.GetColor()) * (isHeader ? 0.5f : 1);

			return Color.FromRgb(255, 255, 255);
		}
	}
}
