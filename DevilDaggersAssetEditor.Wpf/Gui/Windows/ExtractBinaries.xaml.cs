using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Wpf.Extensions;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ExtractBinariesWindow : Window
	{
		private readonly BinaryPathControl _audioControl;
		private readonly BinaryPathControl _coreControl;
		private readonly BinaryPathControl _ddControl;

		private readonly List<BinaryPathControl> _controls = new();

		private string? _outputPath;

		public ExtractBinariesWindow()
		{
			InitializeComponent();

			_audioControl = new(this, "'audio' binary path", BinaryType.Audio, AssetType.Audio);
			_coreControl = new(this, "'core' binary path", BinaryType.Core, AssetType.Shader);
			_ddControl = new(this, "'dd' binary path", BinaryType.Dd, AssetType.Texture);

			_controls.Add(_audioControl);
			_controls.Add(_coreControl);
			_controls.Add(_ddControl);

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
				UpdateButtonExtractBinaries();
			}
		}

		private void TextBoxOutput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_outputPath = TextBoxOutput.Text;
			UpdateButtonExtractBinaries();
		}

		public void UpdateButtonExtractBinaries()
		{
			ButtonExtractBinaries.IsEnabled = !string.IsNullOrWhiteSpace(_outputPath) && Directory.Exists(_outputPath) && _controls.Any(c => c.CheckBoxEnable.IsChecked() && !string.IsNullOrWhiteSpace(c.BinaryPath) && File.Exists(c.BinaryPath));
		}

		private async void ExtractBinaries_Click(object sender, RoutedEventArgs e)
		{
			ButtonExtractBinaries.IsEnabled = false;

			await Task.WhenAll(_controls.Where(c => c.CheckBoxEnable.IsChecked()).Select(c => ExtractBinary(c, _outputPath)));

			ButtonExtractBinaries.IsEnabled = true;

			if (!string.IsNullOrWhiteSpace(_outputPath) && Directory.Exists(_outputPath) && UserHandler.Instance.Settings.OpenModFolderAfterExtracting)
			{
				string? windir = Environment.GetEnvironmentVariable("WINDIR");
				if (windir == null)
					App.Instance.ShowMessage("Environment variable not found", $"Cannot open path \"{_outputPath}\" in Windows Explorer because the WINDIR environment variable was not found.");
				else
					Process.Start(Path.Combine(windir, "explorer.exe"), _outputPath);
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
					string? error = BinaryHandler.ExtractBinary(control.BinaryPath, outputPath, control.Progress);
					if (error == null)
						App.Instance.Dispatcher.Invoke(() => control.Progress.Report("Completed successfully.", 1));
					else
						App.Instance.Dispatcher.Invoke(() => control.Progress.Report(error, 0));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Extracting binary did not complete successfully", $"An error occurred while extracting '{control.BinaryType.ToString().ToLower()}' binary.", ex);
						control.Progress.Report("Execution did not complete successfully.");
					});
				}
			});
		}
	}
}
