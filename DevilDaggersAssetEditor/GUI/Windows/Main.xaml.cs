using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitializeBinaryFileSpecificOptions();
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
					MenuItem audioCompressItem = new MenuItem { Header = "Compress 'audio'" };
					MenuItem audioImportAudioPathsItem = new MenuItem { Header = "Import Audio paths from folder" };
					MenuItem audioImportLoudnessItem = new MenuItem { Header = "Import loudness file" };

					audioCompressItem.Click += (sender, e) => CompressAudio();
					audioImportAudioPathsItem.Click += (sender, e) => AudioAudioTabControl.Handler.ImportFolder();
					audioImportLoudnessItem.Click += (sender, e) => AudioAudioTabControl.Handler.ImportLoudness();

					BinaryFileSpecificOptions.Items.Add(audioCompressItem);
					BinaryFileSpecificOptions.Items.Add(audioImportAudioPathsItem);
					BinaryFileSpecificOptions.Items.Add(audioImportLoudnessItem);
					break;
				case BinaryFileName.DD:
					MenuItem ddCompressItem = new MenuItem { Header = "Compress 'dd'" };
					MenuItem ddImportModelBindingPathsItem = new MenuItem { Header = "Import Model Binding paths from folder" };
					MenuItem ddImportModelPathsItem = new MenuItem { Header = "Import Model paths from folder" };
					MenuItem ddImportShaderPathsItem = new MenuItem { Header = "Import Shader paths from folder" };
					MenuItem ddImportTexturePathsItem = new MenuItem { Header = "Import Texture paths from folder" };

					ddCompressItem.Click += (sender, e) => CompressDD();
					ddImportModelBindingPathsItem.Click += (sender, e) => DDModelBindingsTabControl.Handler.ImportFolder();
					ddImportModelPathsItem.Click += (sender, e) => DDModelsTabControl.Handler.ImportFolder();
					ddImportShaderPathsItem.Click += (sender, e) => DDShadersTabControl.Handler.ImportFolder();
					ddImportTexturePathsItem.Click += (sender, e) => DDTexturesTabControl.Handler.ImportFolder();

					BinaryFileSpecificOptions.Items.Add(ddCompressItem);
					BinaryFileSpecificOptions.Items.Add(ddImportModelBindingPathsItem);
					BinaryFileSpecificOptions.Items.Add(ddImportModelPathsItem);
					BinaryFileSpecificOptions.Items.Add(ddImportShaderPathsItem);
					BinaryFileSpecificOptions.Items.Add(ddImportTexturePathsItem);
					break;
				case BinaryFileName.Core:
					MenuItem coreCompressItem = new MenuItem { Header = "Compress 'core'" };
					MenuItem coreImportShaderPathsItem = new MenuItem { Header = "Import Shader paths from folder" };

					coreCompressItem.Click += (sender, e) => CompressCore();
					coreImportShaderPathsItem.Click += (sender, e) => CoreShadersTabControl.Handler.ImportFolder();

					BinaryFileSpecificOptions.Items.Add(coreCompressItem);
					BinaryFileSpecificOptions.Items.Add(coreImportShaderPathsItem);
					break;
				default:
					throw new Exception($"{nameof(BinaryFileName)} '{activeBinaryFileName}' has not been implemented in this method.");
			}
		}

		private void CompressAudio()
		{
			if (!AudioAudioTabControl.Handler.IsComplete())
			{
				MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (promptResult == MessageBoxResult.No)
					return;
			}

			Compress(AudioAudioTabControl.Handler.Assets.Cast<AbstractAsset>().ToList(), BinaryFileName.Audio);
		}

		private void CompressDD()
		{
			if (!DDModelBindingsTabControl.Handler.IsComplete()
			 || !DDModelsTabControl.Handler.IsComplete()
			 || !DDShadersTabControl.Handler.IsComplete()
			 || !DDTexturesTabControl.Handler.IsComplete())
			{
				MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (promptResult == MessageBoxResult.No)
					return;
			}

			Compress(
				DDModelBindingsTabControl.Handler.Assets.Cast<AbstractAsset>()
				.Concat(DDModelsTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(DDShadersTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(DDTexturesTabControl.Handler.Assets.Cast<AbstractAsset>()).ToList(),
				BinaryFileName.DD);
		}

		private void CompressCore()
		{
			if (!CoreShadersTabControl.Handler.IsComplete())
			{
				MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (promptResult == MessageBoxResult.No)
					return;
			}

			Compress(CoreShadersTabControl.Handler.Assets.Cast<AbstractAsset>().ToList(), BinaryFileName.Core);
		}

		private void Compress(List<AbstractAsset> assets, BinaryFileName binaryFileName)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				InitialDirectory = Path.Combine(Utils.DDFolder, binaryFileName.GetSubfolderName())
			};
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			Compressor.Compress(assets, dialog.FileName, binaryFileName);
		}
	}
}