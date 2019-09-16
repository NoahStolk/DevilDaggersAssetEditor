using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public BinaryFileName activeBinaryFileName;

		public MainWindow()
		{
			InitializeComponent();

			foreach (BinaryFileName binaryFileName in (BinaryFileName[])Enum.GetValues(typeof(BinaryFileName)))
			{
				MenuItem extractItem = new MenuItem { Header = binaryFileName.ToString().ToLower(), IsEnabled = binaryFileName != BinaryFileName.Particle };
				extractItem.Click += (sender, e) => Extract_Click(binaryFileName);
				ExtractMenuItem.Items.Add(extractItem);
			}

			DispatcherTimer timer = new DispatcherTimer();
			timer.Tick += (sender, e) =>
			{
				InitializeBinaryFileSpecificOptions();
				timer.Stop();
			};
			timer.Start();
		}

		private void Extract_Click(BinaryFileName binaryFileName)
		{
			OpenFileDialog openDialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			using (CommonOpenFileDialog saveDialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Utils.DDFolder })
			{
				CommonFileDialogResult saveResult = saveDialog.ShowDialog();
				if (saveResult == CommonFileDialogResult.Ok)
					Extractor.Extract(openDialog.FileName, saveDialog.FileName, binaryFileName);
			}
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InitializeBinaryFileSpecificOptions();
		}

		private void InitializeBinaryFileSpecificOptions()
		{
			Enum.TryParse((TabControl.Items[TabControl.SelectedIndex] as TabItem).Header.ToString(), true, out activeBinaryFileName);

			BinaryFileSpecificOptions.Header = activeBinaryFileName.ToString();
			BinaryFileSpecificOptions.Items.Clear();

			switch (activeBinaryFileName)
			{
				case BinaryFileName.Audio:
					MenuItem audioCompressItem = new MenuItem { Header = "Compress audio" };
					MenuItem audioImportPathsItem = new MenuItem { Header = "Import audio paths from folder" };
					MenuItem audioImportLoudnessItem = new MenuItem { Header = "Import loudness file" };

					audioCompressItem.Click += AudioCompressItem_Click;
					audioImportPathsItem.Click += AudioImportPathsItem_Click;
					audioImportLoudnessItem.Click += AudioImportLoudnessItem_Click;

					BinaryFileSpecificOptions.Items.Add(audioCompressItem);
					BinaryFileSpecificOptions.Items.Add(audioImportPathsItem);
					BinaryFileSpecificOptions.Items.Add(audioImportLoudnessItem);
					break;
				default:
					throw new Exception($"{nameof(BinaryFileName)} '{activeBinaryFileName}' has not been implemented in this method.");
			}
		}

		private void AudioCompressItem_Click(object sender, RoutedEventArgs e)
		{
			AudioTabControl.Compress();
		}

		private void AudioImportPathsItem_Click(object sender, RoutedEventArgs e)
		{
			AudioTabControl.ImportFolder();
		}

		private void AudioImportLoudnessItem_Click(object sender, RoutedEventArgs e)
		{
			AudioTabControl.ImportLoudness();
		}
	}
}