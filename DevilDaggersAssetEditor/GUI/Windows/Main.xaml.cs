using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
					MenuItem audioImportAudioPathsItem = new MenuItem { Header = "Import Audio paths from folder" };
					MenuItem audioImportLoudnessItem = new MenuItem { Header = "Import loudness file" };

					audioCompressItem.Click += (sender, e) => AudioAudioTabControl.Handler.Compress();
					audioImportAudioPathsItem.Click += (sender, e) => AudioAudioTabControl.Handler.ImportFolder();
					audioImportLoudnessItem.Click += (sender, e) => AudioAudioTabControl.Handler.ImportLoudness();

					BinaryFileSpecificOptions.Items.Add(audioCompressItem);
					BinaryFileSpecificOptions.Items.Add(audioImportAudioPathsItem);
					BinaryFileSpecificOptions.Items.Add(audioImportLoudnessItem);
					break;
				case BinaryFileName.DD:
					MenuItem ddCompressItem = new MenuItem { Header = "Compress DD" };
					MenuItem ddImportModelBindingPathsItem = new MenuItem { Header = "Import Model Binding paths from folder" };

					ddCompressItem.Click += (sender, e) => DDModelBindingsTabControl.Handler.Compress();
					ddImportModelBindingPathsItem.Click += (sender, e) => DDModelBindingsTabControl.Handler.ImportFolder();

					BinaryFileSpecificOptions.Items.Add(ddCompressItem);
					BinaryFileSpecificOptions.Items.Add(ddImportModelBindingPathsItem);
					break;
				default:
					throw new Exception($"{nameof(BinaryFileName)} '{activeBinaryFileName}' has not been implemented in this method.");
			}
		}
	}
}