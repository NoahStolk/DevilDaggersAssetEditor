using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileAnalyzer;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Utils;
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
				byte[] sourceFileBytes = File.ReadAllBytes(openDialog.FileName);

				AnalyzerFileResult? result = TryReadResourceFile(openDialog.FileName, sourceFileBytes) ?? TryReadParticleFile(openDialog.FileName, sourceFileBytes);
				if (result == null)
					App.Instance.ShowMessage("File not recognized", "Make sure to open one of the following binary files: audio, core, dd, particle");
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

			IEnumerable<IGrouping<AssetType, IChunk>> chunksByType = fileResult.Chunks.GroupBy(c => c.AssetType);
			foreach (IGrouping<AssetType, IChunk> group in chunksByType.OrderBy(c => c.Key))
			{
				IEnumerable<IChunk> validChunks = group;
				uint size = 0;
				uint headerSize = 0;
				foreach (IChunk chunk in validChunks)
				{
					if (chunk is ModelChunk)
					{
						size += chunk.Size - 10;
						headerSize += 10;
					}
					else if (chunk is ShaderChunk)
					{
						size += chunk.Size - 12;
						headerSize += 12;
					}
					else if (chunk is TextureChunk)
					{
						size += chunk.Size - 11;
						headerSize += 11;
					}
					else if (chunk is ParticleChunk particleChunk)
					{
						size += ParticleFileHandler.ParticleBufferLength;
						headerSize += (uint)particleChunk.Name.Length;
					}
					else
					{
						size += chunk.Size;
					}
				}

				chunkGroups.Add(group.Key.ToString(), ChunkResult(GetColor(group.Key, false), size, validChunks.ToList()));
				if (headerSize > 0)
					chunkGroups.Add($"{group.Key} header", ChunkResult(GetColor(group.Key, true), headerSize, validChunks.ToList()));
			}

			uint unknownSize = (uint)(fileResult.FileByteCount - chunkGroups.Sum(c => c.Value.ByteCount));
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
				Color color = ChunkResultColor(kvp.Value);

				double sizePercentage = kvp.Value.ByteCount / (double)fileResult.FileByteCount;
				double width = sizePercentage * Canvas.Width;
				Rectangle rect = new()
				{
					Width = width,
					Height = totalHeight,
					Fill = new SolidColorBrush(color),
				};
				Canvas.SetLeft(rect, pos);
				Canvas.Children.Add(rect);

				pos += width;

				Rectangle rectColor = new() { Fill = new SolidColorBrush(color) };
				rectColor.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
				rectColor.StrokeThickness = 1;
				rectColor.SnapsToDevicePixels = true;

				SolidColorBrush bgColor = ColorUtils.ThemeColors[i++ % 2 == 0 ? "Gray4" : "Gray5"];

				Rectangle rectBackground = new() { Fill = bgColor };
				rectBackground.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
				rectBackground.StrokeThickness = 1;
				rectBackground.StrokeDashArray = new DoubleCollection(new List<double> { 1, 2 });
				rectBackground.SnapsToDevicePixels = true;
				Label labelPercentage = new() { Content = sizePercentage.ToString("0.000%") };

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
			foreach (IChunk chunk in fileResult.Chunks.OrderByDescending(c => c.Size))
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

				Border border = new() { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)) };
				border.Child = stackPanel;
				Grid.SetColumn(border, k % _columnCount);
				Grid.SetRow(border, k / _columnCount);

				ChunkData.Children.Add(border);
				k++;
			}
		}

		private static Brush GetTextColorBasedOnBackgroundColor(Color backgroundColor)
			=> ColorUtils.GetPerceivedBrightness(backgroundColor) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];

		private static AnalyzerChunkGroup ChunkResult(Color color, uint byteCount, List<IChunk> chunks)
			=> new(color.R, color.G, color.B, byteCount, chunks);

		private static Color ChunkResultColor(AnalyzerChunkGroup chunkResult)
			=> Color.FromRgb(chunkResult.R, chunkResult.G, chunkResult.B);

		private static AnalyzerFileResult? TryReadResourceFile(string sourceFileName, byte[] sourceFileBytes)
		{
			try
			{
				ResourceFileHandler fileHandler = new(BinaryFileType.None); // Since we're only validating the file, we can pass None as BinaryFileType.
				fileHandler.ValidateFile(sourceFileBytes);

				byte[] tocBuffer = ResourceFileHandler.ReadTocBuffer(sourceFileBytes);

				return new(sourceFileName, (uint)sourceFileBytes.Length, (uint)tocBuffer.Length + ResourceFileHandler.HeaderSize, ResourceFileHandler.ReadChunks(tocBuffer).Cast<IChunk>().ToList());
			}
			catch
			{
				return null;
			}
		}

		private static AnalyzerFileResult? TryReadParticleFile(string sourceFileName, byte[] sourceFileBytes)
		{
			try
			{
				ParticleFileHandler fileHandler = new();
				fileHandler.ValidateFile(sourceFileBytes);

				int i = ParticleFileHandler.HeaderSize;
				List<IChunk> chunks = new List<IChunk>();
				while (i < sourceFileBytes.Length)
				{
					ParticleChunk chunk = ParticleFileHandler.ReadParticleChunk(sourceFileBytes, i);
					i += chunk.Buffer.Length;
					chunks.Add(chunk);
				}

				return new(sourceFileName, (uint)sourceFileBytes.Length, ParticleFileHandler.HeaderSize, chunks);
			}
			catch
			{
				return null;
			}
		}

		private static Color GetColor(AssetType? assetType, bool isHeader)
		{
			if (assetType.HasValue)
				return EditorUtils.FromRgbTuple(assetType.Value.GetColor()) * (isHeader ? 0.5f : 1);

			return Color.FromRgb(255, 255, 255);
		}
	}
}
