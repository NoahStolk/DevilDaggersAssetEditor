using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersCore.Wpf.Extensions;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ExtractBinariesWindow : Window
	{
		private readonly BinaryPathControl _audioControl = new("'audio' binary path", BinaryFileType.Audio, AssetType.Audio);
		private readonly BinaryPathControl _coreControl = new("'core' binary path", BinaryFileType.Core, AssetType.Shader);
		private readonly BinaryPathControl _ddControl = new("'dd' binary path", BinaryFileType.Dd, AssetType.Texture);
		private readonly BinaryPathControl _particleControl = new("'particle' binary path", BinaryFileType.Particle, AssetType.Particle);

		private readonly List<BinaryPathControl> _controls = new();

		private string? _outputPath;

		public ExtractBinariesWindow()
		{
			InitializeComponent();

			_controls.Add(_audioControl);
			_controls.Add(_coreControl);
			_controls.Add(_ddControl);
			_controls.Add(_particleControl);

			for (int i = 0; i < _controls.Count; i++)
				Main.Children.Insert(i, _controls[i]);
		}

		private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog folderDialog = new();
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

			await Task.WhenAll(_controls.Where(c => c.CheckBoxEnable.IsChecked()).Select(c => ExtractBinary(c, _outputPath)));

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

		private static async Task ExtractBinary(BinaryPathControl control, string? outputPath)
		{
			if (string.IsNullOrWhiteSpace(outputPath) || string.IsNullOrWhiteSpace(control.BinaryPath) || !Directory.Exists(outputPath) || !File.Exists(control.BinaryPath))
				return;

			await Task.Run(() =>
			{
				try
				{
					IBinaryFileHandler fileHandler = control.BinaryFileType switch
					{
						BinaryFileType.Particle => new ParticleFileHandler(),
						_ => new ResourceFileHandler(control.BinaryFileType),
					};

					fileHandler.ExtractBinary(control.BinaryPath, outputPath, control.Progress);
					App.Instance.Dispatcher.Invoke(() => control.Progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Extracting binary did not complete successfully", $"An error occurred while extracting '{control.BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' binary.", ex);
						control.Progress.Report("Execution did not complete successfully.");
					});
				}
			});
		}
	}
}
