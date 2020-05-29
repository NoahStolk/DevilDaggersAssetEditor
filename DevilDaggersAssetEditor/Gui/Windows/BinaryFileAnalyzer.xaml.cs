using DevilDaggersAssetCore;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.Chunks;
using DevilDaggersAssetCore.Info;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Code;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SPath = System.IO.Path;

namespace DevilDaggersAssetEditor.Gui.Windows
{
	public partial class BinaryFileAnalyzerWindow : Window
	{
		public string fileName;
		public uint fileByteCount;
		public uint headerByteCount;
		public List<AbstractChunk> chunks;

		public BinaryFileAnalyzerWindow()
		{
			InitializeComponent();
		}

		private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			if (UserHandler.Instance.settings.EnableDevilDaggersRootFolder)
				openDialog.InitialDirectory = SPath.Combine(UserHandler.Instance.settings.DevilDaggersRootFolder);

			bool? openResult = openDialog.ShowDialog();
			if (openResult.HasValue && openResult.Value)
			{
				ClearGui();

				byte[] sourceFileBytes = File.ReadAllBytes(openDialog.FileName);

				if (!TryReadResourceFile(openDialog.FileName, sourceFileBytes) && !TryReadParticleFile(openDialog.FileName, sourceFileBytes))
					MessageBox.Show("File not recognized. Make sure to open one of the following binary files: audio, core, dd, particle");
				else
					UpdateGui();
			}
		}

		private bool TryReadResourceFile(string sourceFileName, byte[] sourceFileBytes)
		{
			try
			{
				fileName = sourceFileName;
				fileByteCount = (uint)sourceFileBytes.Length;

				ResourceFileHandler fileHandler = new ResourceFileHandler(BinaryFileType.Audio | BinaryFileType.Core | BinaryFileType.Dd);
				fileHandler.ValidateFile(sourceFileBytes);

				byte[] tocBuffer = fileHandler.ReadTocBuffer(sourceFileBytes);
				headerByteCount = (uint)tocBuffer.Length + ResourceFileHandler.HeaderSize;

				chunks = fileHandler.ReadChunks(tocBuffer);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private bool TryReadParticleFile(string sourceFileName, byte[] sourceFileBytes)
		{
			try
			{
				fileName = sourceFileName;
				fileByteCount = (uint)sourceFileBytes.Length;
				headerByteCount = ParticleFileHandler.HeaderSize;

				ParticleFileHandler fileHandler = new ParticleFileHandler();
				fileHandler.ValidateFile(sourceFileBytes);

				int i = (int)headerByteCount;
				chunks = new List<AbstractChunk>();
				while (i < sourceFileBytes.Length)
				{
					ParticleChunk chunk = fileHandler.ReadParticleChunk(sourceFileBytes, i);
					i += chunk.Name.Length;
					i += chunk.Buffer.Length;
					chunks.Add(chunk);
				}
				return true;
			}
			catch
			{
				return false;
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
				_ => Color.FromRgb(255, 255, 255)
			};
		}

		private void ClearGui()
		{
			FileName.Content = string.Empty;
			FileSize.Content = string.Empty;
			Canvas.Children.Clear();
			Data.ColumnDefinitions.Clear();
			Data.Children.Clear();
			Separator.Visibility = Visibility.Hidden;
		}

		private void UpdateGui()
		{
			Dictionary<string, (Color color, uint byteCount, uint chunkCount)> chunkInfos = new Dictionary<string, (Color color, uint byteCount, uint chunkCount)>
			{
				{ "File header", (GetColor("File header"), headerByteCount, 0) }
			};

			IEnumerable<IGrouping<string, AbstractChunk>> chunksByType = chunks.GroupBy(c => ChunkInfo.All.FirstOrDefault(ci => ci.ChunkType == c.GetType()).DataName);
			foreach (IGrouping<string, AbstractChunk> group in chunksByType.OrderBy(c => c.Key))
			{
				IEnumerable<AbstractChunk> validChunks = group.Where(c => c.Size != 0); // Filter empty chunks (garbage in core file TOC buffer).
				uint size = 0;
				uint headerSize = 0;
				foreach (AbstractChunk chunk in validChunks)
				{
					if (chunk is ModelChunk || chunk is ShaderChunk || chunk is TextureChunk)
					{
						uint headerChunkSize = ChunkInfo.All.FirstOrDefault(c => c.ChunkType == chunk.GetType()).HeaderInfo.FixedSize.Value;

						size += chunk.Size - headerChunkSize;
						headerSize += headerChunkSize;
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

				chunkInfos.Add(group.Key, (GetColor(group.Key), size, (uint)validChunks.Count()));
				if (headerSize > 0)
				{
					string headerKey = $"{group.Key} header";
					chunkInfos.Add(headerKey, (GetColor(headerKey), headerSize, (uint)validChunks.Count()));
				}
			}

			uint unknownSize = (uint)(fileByteCount - chunkInfos.Sum(c => c.Value.byteCount));
			if (unknownSize > 0)
				chunkInfos.Add("Unknown", (GetColor("Unknown"), unknownSize, 0));

			int totalWidth = 1280;
			int totalHeight = 32;
			int pos = 0;
			int col = 0;

			FileName.Content = fileName;
			FileSize.Content = $"{fileByteCount:N0} bytes";
			Separator.Visibility = Visibility.Visible;
			foreach (KeyValuePair<string, (Color color, uint byteCount, uint chunkCount)> chunkInfo in chunkInfos)
			{
				float sizePercentage = chunkInfo.Value.byteCount / (float)fileByteCount;
				int width = (int)(sizePercentage * totalWidth);
				Rectangle rect = new Rectangle
				{
					Width = width,
					Height = totalHeight,
					Fill = new SolidColorBrush(chunkInfo.Value.color)
				};
				Canvas.SetLeft(rect, pos);
				Canvas.Children.Add(rect);

				pos += width;

				Data.ColumnDefinitions.Add(new ColumnDefinition());
				StackPanel stackPanel = new StackPanel { Background = new SolidColorBrush(GetColor(chunkInfo.Key)) };
				Grid.SetColumn(stackPanel, col++);
				stackPanel.Children.Add(new Label { Content = $"{chunkInfo.Key} data ({sizePercentage:0.000%})", FontWeight = FontWeights.Bold, Height = 40 });
				stackPanel.Children.Add(new Label { Content = $"{chunkInfo.Value.byteCount:N0} bytes" });
				if (chunkInfo.Value.chunkCount > 0)
					stackPanel.Children.Add(new Label { Content = $"{chunkInfo.Value.chunkCount} chunks" });
				Data.Children.Add(stackPanel);
			}
		}
	}
}