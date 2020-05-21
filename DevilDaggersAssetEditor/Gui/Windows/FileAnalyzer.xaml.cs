using DevilDaggersAssetCore;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.Chunks;
using DevilDaggersAssetCore.User;
using Microsoft.Win32;
using System;
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
	public partial class FileAnalyzerWindow : Window
	{
		public uint fileByteCount;
		public uint headerByteCount;
		public List<AbstractChunk> chunks;

		public FileAnalyzerWindow()
		{
			InitializeComponent();
		}

		private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			if (UserHandler.Instance.settings.EnableDevilDaggersRootFolder)
				openDialog.InitialDirectory = SPath.Combine(UserHandler.Instance.settings.DevilDaggersRootFolder, "res");

			bool? openResult = openDialog.ShowDialog();
			if (openResult.HasValue && openResult.Value)
			{
				byte[] sourceFileBytes = File.ReadAllBytes(openDialog.FileName);
				fileByteCount = (uint)sourceFileBytes.Length;

				// TODO: Particle
				ResourceFileHandler fileHandler = new ResourceFileHandler(BinaryFileType.Audio | BinaryFileType.Core | BinaryFileType.Dd);
				fileHandler.ValidateFile(sourceFileBytes);

				byte[] tocBuffer = fileHandler.ReadTocBuffer(sourceFileBytes);
				headerByteCount = (uint)tocBuffer.Length + 12; // 12 for first 12 bytes in resource file

				chunks = fileHandler.ReadChunks(tocBuffer);

				UpdateGui();
			}
		}

		private string ChunkTypeToString(Type type)
		{
			if (type == typeof(AudioChunk)) return "Audio";
			if (type == typeof(ModelBindingChunk)) return "Model binding";
			if (type == typeof(ModelChunk)) return "Model";
			if (type == typeof(ShaderChunk)) return "Shader";
			if (type == typeof(TextureChunk)) return "Texture";
			return null;
		}

		private static Color GetColor(string type) => type switch
		{
			"Audio" => Color.FromRgb(255, 0, 255),
			"Model binding" => Color.FromRgb(0, 255, 255),
			"Model" => Color.FromRgb(255, 0, 0),
			"Shader" => Color.FromRgb(0, 255, 0),
			"Texture" => Color.FromRgb(255, 127, 0),
			"Particle" => Color.FromRgb(255, 255, 0),
			"Header" => Color.FromRgb(255, 127, 127),
			"Unknown" => Color.FromRgb(127, 127, 255),
			_ => Color.FromRgb(255, 255, 255)
		};

		private void UpdateGui()
		{
			Dictionary<string, (Color color, uint byteCount)> sizeInfos = new Dictionary<string, (Color color, uint byteCount)>
			{
				{ "Header", (GetColor("Header"), headerByteCount) }
			};

			IEnumerable<IGrouping<string, AbstractChunk>> chunksByType = chunks.GroupBy(c => ChunkTypeToString(c.GetType()));
			foreach (IGrouping<string, AbstractChunk> group in chunksByType.OrderBy(c => c.Key))
				sizeInfos.Add(group.Key, (GetColor(group.Key), (uint)group.Sum(c => c.Size)));

			uint unknownSize = (uint)(fileByteCount - headerByteCount - chunksByType.Sum(c => c.Sum(c => c.Size)));
			if (unknownSize > 0)
				sizeInfos.Add("Unknown", (GetColor("Unknown"), unknownSize));

			int totalWidth = 1280;
			int totalHeight = 32;
			int pos = 0;
			int col = 0;

			FileSize.Content = $"{fileByteCount:N0} bytes";
			C.Children.Clear();
			Data.ColumnDefinitions.Clear();
			foreach (KeyValuePair<string, (Color color, uint byteCount)> sizeInfo in sizeInfos)
			{
				float sizePercentage = sizeInfo.Value.byteCount / (float)fileByteCount;
				int width = (int)(sizePercentage * totalWidth);
				Rectangle rect = new Rectangle
				{
					Width = width,
					Height = totalHeight,
					Fill = new SolidColorBrush(sizeInfo.Value.color)
				};
				Canvas.SetLeft(rect, pos);
				C.Children.Add(rect);

				pos += width;

				Data.ColumnDefinitions.Add(new ColumnDefinition());
				Label label = new Label { Content = $"{sizeInfo.Key} data: {sizeInfo.Value.byteCount:N0} bytes ({sizePercentage:0.000%})", Background = new SolidColorBrush(GetColor(sizeInfo.Key)) };
				Grid.SetColumn(label, col++);
				Data.Children.Add(label);
			}
		}
	}
}