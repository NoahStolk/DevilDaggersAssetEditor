using DevilDaggersAssetEditor.BinaryFileAnalyzer;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class BinaryFileAnalyzerWindow : Window
	{
		public BinaryFileAnalyzerWindow(AnalyzerFileResult fileResult)
		{
			InitializeComponent();

			Dictionary<string, AnalyzerChunkGroup> chunkInfos = new Dictionary<string, AnalyzerChunkGroup>
			{
				{ "File header", ChunkResult(GetColor("File header"), fileResult.headerByteCount, Array.Empty<AbstractChunk>()) },
			};

			IEnumerable<IGrouping<string, AbstractChunk>> chunksByType = fileResult.chunks.GroupBy(c => ChunkInfo.All.FirstOrDefault(ci => ci.ChunkType == c.GetType()).DataName);
			foreach (IGrouping<string, AbstractChunk> group in chunksByType.OrderBy(c => c.Key))
			{
				IEnumerable<AbstractChunk> validChunks = group;
				uint size = 0;
				uint headerSize = 0;
				foreach (AbstractChunk chunk in validChunks)
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

				chunkInfos.Add(group.Key, ChunkResult(GetColor(group.Key), size, validChunks.ToArray()));
				if (headerSize > 0)
				{
					string headerKey = $"{group.Key} header";
					chunkInfos.Add(headerKey, ChunkResult(GetColor(headerKey), headerSize, validChunks.ToArray()));
				}
			}

			uint unknownSize = (uint)(fileResult.fileByteCount - chunkInfos.Sum(c => c.Value.byteCount));
			if (unknownSize > 0)
				chunkInfos.Add("Unknown", ChunkResult(GetColor("Unknown"), unknownSize, Array.Empty<AbstractChunk>()));

			int totalHeight = 32;
			double pos = 0;

			FileName.Content = fileResult.fileName;
			FileSize.Content = $"{fileResult.fileByteCount:N0} bytes";
			float maxPercentage = chunkInfos.Max(c => c.Value.byteCount / (float)fileResult.fileByteCount);
			int i = 0;
			foreach (KeyValuePair<string, AnalyzerChunkGroup> kvp in chunkInfos.OrderBy(c => c.Value.chunks.Length != 0).ThenByDescending(c => c.Value.byteCount).Where(c => c.Value.byteCount > 0))
			{
				Color color = ChunkResultColor(kvp.Value);

				double sizePercentage = kvp.Value.byteCount / (double)fileResult.fileByteCount;
				double width = sizePercentage * Canvas.Width;
				Rectangle rect = new Rectangle
				{
					Width = width,
					Height = totalHeight,
					Fill = new SolidColorBrush(color),
				};
				Canvas.SetLeft(rect, pos);
				Canvas.Children.Add(rect);

				pos += width;

				Rectangle rectColor = new Rectangle { Fill = new SolidColorBrush(color) };
				rectColor.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
				rectColor.StrokeThickness = 1;
				rectColor.SnapsToDevicePixels = true;

				SolidColorBrush bgColor = ColorUtils.ThemeColors[i++ % 2 == 0 ? "Gray4" : "Gray5"];

				Rectangle rectBackground = new Rectangle { Fill = bgColor };
				rectBackground.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
				rectBackground.StrokeThickness = 1;
				rectBackground.StrokeDashArray = new DoubleCollection(new List<double> { 1, 2 });
				rectBackground.SnapsToDevicePixels = true;
				Label labelPercentage = new Label { Content = sizePercentage.ToString("0.000%", CultureInfo.InvariantCulture) };

				Grid.SetColumn(rectColor, 0);
				Grid.SetColumn(rectBackground, 1);
				Grid.SetColumnSpan(labelPercentage, 2);

				Grid grid = new Grid();
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

				StackPanel stackPanel = new StackPanel { Background = bgColor };
				stackPanel.Children.Add(new Label { Content = $"{kvp.Key} data", FontWeight = FontWeights.Bold });
				stackPanel.Children.Add(grid);
				stackPanel.Children.Add(new Label { Content = $"{kvp.Value.byteCount:N0} bytes" });
				if (kvp.Value.chunks.Length > 0)
					stackPanel.Children.Add(new Label { Content = $"{kvp.Value.chunks.Length} chunks" });

				Data.Children.Add(stackPanel);
			}

			int columns = 8;
			for (int j = 0; j < columns; j++)
				ChunkData.ColumnDefinitions.Add(new ColumnDefinition());

			int k = 0;
			foreach (AbstractChunk chunk in fileResult.chunks.OrderByDescending(c => c.Size))
			{
				if (k % columns == 0)
					ChunkData.RowDefinitions.Add(new RowDefinition());

				double chunkSizePercentage = chunk.Size / (double)fileResult.fileByteCount;

				string dataName = ChunkInfo.All.FirstOrDefault(c => c.ChunkType == chunk.GetType()).DataName;
				StackPanel stackPanel = new StackPanel { Background = new SolidColorBrush(GetColor(dataName) * (float)chunkSizePercentage * 50) };
				TextBlock textBlockDataName = new TextBlock { Margin = new Thickness(2), Text = dataName, FontWeight = FontWeights.Bold };
				if (chunkSizePercentage < 0.005f)
					textBlockDataName.Background = new SolidColorBrush(GetColor(dataName) * 0.25f);
				stackPanel.Children.Add(textBlockDataName);
				stackPanel.Children.Add(new Label
				{
					Content = chunk.Name,
					FontWeight = FontWeights.Bold,
				});
				stackPanel.Children.Add(new Label { Content = $"{chunk.Size:N0} bytes" });
				stackPanel.Children.Add(new Label { Content = $"{chunkSizePercentage:0.000%} of file" });

				Border border = new Border
				{
					BorderThickness = new Thickness(1),
					BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
				};
				border.Child = stackPanel;
				Grid.SetColumn(border, k % columns);
				Grid.SetRow(border, k / columns);

				ChunkData.Children.Add(border);
				k++;
			}
		}

		private static AnalyzerChunkGroup ChunkResult(Color color, uint byteCount, AbstractChunk[] chunks)
			=> new AnalyzerChunkGroup(color.R, color.G, color.B, byteCount, chunks);

		private static Color ChunkResultColor(AnalyzerChunkGroup chunkResult)
			=> Color.FromRgb(chunkResult.r, chunkResult.g, chunkResult.b);

		public static AnalyzerFileResult? TryReadResourceFile(string sourceFileName, byte[] sourceFileBytes)
		{
			try
			{
				ResourceFileHandler fileHandler = new ResourceFileHandler(BinaryFileType.Audio | BinaryFileType.Core | BinaryFileType.Dd);
				fileHandler.ValidateFile(sourceFileBytes);

				byte[] tocBuffer = ResourceFileHandler.ReadTocBuffer(sourceFileBytes);

				return new AnalyzerFileResult(sourceFileName, (uint)sourceFileBytes.Length, (uint)tocBuffer.Length + ResourceFileHandler.HeaderSize, ResourceFileHandler.ReadChunks(tocBuffer));
			}
			catch
			{
				return null;
			}
		}

		public static AnalyzerFileResult? TryReadParticleFile(string sourceFileName, byte[] sourceFileBytes)
		{
			try
			{
				ParticleFileHandler fileHandler = new ParticleFileHandler();
				fileHandler.ValidateFile(sourceFileBytes);

				int i = ParticleFileHandler.HeaderSize;
				List<AbstractChunk> chunks = new List<AbstractChunk>();
				while (i < sourceFileBytes.Length)
				{
					ParticleChunk chunk = ParticleFileHandler.ReadParticleChunk(sourceFileBytes, i);
					i += chunk.Name.Length;
					i += chunk.Buffer.Length;
					chunks.Add(chunk);
				}

				return new AnalyzerFileResult(sourceFileName, (uint)sourceFileBytes.Length, ParticleFileHandler.HeaderSize, chunks);
			}
			catch
			{
				return null;
			}
		}

		private static Color GetColor(string type)
		{
			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.DataName == type);
			if (chunkInfo != null)
				return chunkInfo.GetColor();

			chunkInfo = ChunkInfo.All.FirstOrDefault(c => $"{c.DataName} header" == type);
			if (chunkInfo != null)
				return chunkInfo.GetColor() * 0.5f;

			return type switch
			{
				"File header" => Color.FromRgb(255, 127, 127),
				"Unknown" => Color.FromRgb(127, 127, 255),
				_ => Color.FromRgb(255, 255, 255),
			};
		}
	}
}