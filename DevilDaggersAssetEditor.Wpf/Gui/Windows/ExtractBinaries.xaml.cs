using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ExtractBinariesWindow : Window
	{
		private readonly BinaryPathControl _audioControl = new BinaryPathControl("'audio' binary path", BinaryFileType.Audio, AssetType.Audio);
		private readonly BinaryPathControl _coreControl = new BinaryPathControl("'core' binary path", BinaryFileType.Core, AssetType.Shader);
		private readonly BinaryPathControl _ddControl = new BinaryPathControl("'dd' binary path", BinaryFileType.Dd, AssetType.Texture);
		private readonly BinaryPathControl _particleControl = new BinaryPathControl("'particle' binary path", BinaryFileType.Particle, AssetType.Particle);

		private string? _outputPath;

		public ExtractBinariesWindow()
		{
			InitializeComponent();

			Main.Children.Insert(0, _audioControl);
			Main.Children.Insert(1, _coreControl);
			Main.Children.Insert(2, _ddControl);
			Main.Children.Insert(3, _particleControl);
		}

		private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
			folderDialog.OpenAssetsRootFolder();

			if (folderDialog.ShowDialog() == true)
			{
				_outputPath = folderDialog.SelectedPath;
				TextBoxOutput.Text = _outputPath;
			}
		}

		private void TextBoxOutput_TextChanged(object sender, TextChangedEventArgs e)
			=> _outputPath = TextBoxOutput.Text;

		private async void ExtractBinaries_Click(object sender, RoutedEventArgs e)
		{
			ButtonExtractBinaries.IsEnabled = false;

			await Task.WhenAll(new List<Task>
			{
				ExtractBinary(BinaryFileType.Audio, _audioControl.BinaryPath, _outputPath, _audioControl.Progress),
				ExtractBinary(BinaryFileType.Core, _coreControl.BinaryPath, _outputPath, _coreControl.Progress),
				ExtractBinary(BinaryFileType.Dd, _ddControl.BinaryPath, _outputPath, _ddControl.Progress),
				ExtractBinary(BinaryFileType.Particle, _particleControl.BinaryPath, _outputPath, _particleControl.Progress),
			});

			ButtonExtractBinaries.IsEnabled = true;

			if (!string.IsNullOrWhiteSpace(_outputPath) && Directory.Exists(_outputPath))
			{
				if (UserHandler.Instance.Settings.CreateModFileWhenExtracting)
					ModFileUtils.CreateModFileFromPath(_outputPath);

				if (UserHandler.Instance.Settings.OpenModFolderAfterExtracting)
				{
					string? windir = Environment.GetEnvironmentVariable("WINDIR");
					if (windir == null)
						App.Instance.ShowMessage("Environment variable not found", $"Cannot open path \"{_outputPath}\" in Windows Explorer because the WINDIR environment variable was not found.");
					else
						Process.Start(Path.Combine(windir, "explorer.exe"), _outputPath);
				}
			}
		}

		private static async Task ExtractBinary(BinaryFileType binaryFileType, string? inputPath, string? outputPath, ProgressWrapper progress)
		{
			if (string.IsNullOrWhiteSpace(outputPath) || string.IsNullOrWhiteSpace(inputPath) || !Directory.Exists(outputPath) || !File.Exists(inputPath))
				return;

			await Task.Run(() =>
			{
				try
				{
					IBinaryFileHandler fileHandler = binaryFileType switch
					{
						BinaryFileType.Particle => new ParticleFileHandler(),
						_ => new ResourceFileHandler(binaryFileType),
					};

					fileHandler.ExtractBinary(inputPath, outputPath, progress);
					App.Instance.Dispatcher.Invoke(() => progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Extracting binary did not complete successfully", $"An error occurred while extracting '{binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' binary.", ex);
						progress.Report("Execution did not complete successfully.");
					});
				}
			});
		}
	}
}