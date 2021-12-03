using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersCore.Extensions;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Wpf.Extensions;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows;

public partial class ExtractBinariesWindow : Window
{
	private readonly BinaryPathControl _audioControl;
	private readonly BinaryPathControl _coreControl;
	private readonly BinaryPathControl _ddControl;

	private readonly List<BinaryPathControl> _controls = new();

	private string? _outputPath;
	private string? _modFilePath;

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

	private void BrowseModFilePathButton_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new() { Filter = GuiUtils.ModFileFilter };
		saveFileDialog.OpenModsRootFolder();

		if (saveFileDialog.ShowDialog() == true)
		{
			_modFilePath = saveFileDialog.FileName;
			TextBoxModFilePath.Text = _modFilePath;
		}
	}

	private void TextBoxOutput_TextChanged(object sender, TextChangedEventArgs e)
	{
		_outputPath = TextBoxOutput.Text;
		UpdateButtonExtractBinaries();
	}

	private void TextBoxModFilePath_TextChanged(object sender, TextChangedEventArgs e)
	{
		_modFilePath = TextBoxModFilePath.Text;
	}

	public void UpdateButtonExtractBinaries()
	{
		ButtonExtractBinaries.IsEnabled = !string.IsNullOrWhiteSpace(_outputPath) && Directory.Exists(_outputPath) && _controls.Any(c => c.CheckBoxEnable.IsChecked() && !string.IsNullOrWhiteSpace(c.BinaryPath) && File.Exists(c.BinaryPath));
	}

	private async void ExtractBinaries_Click(object sender, RoutedEventArgs e)
	{
		ButtonExtractBinaries.IsEnabled = false;

		await Task.WhenAll(_controls.Where(c => c.CheckBoxEnable.IsChecked()).Select(c => ExtractBinary(c)));

		CreateModFile();

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

	private async Task ExtractBinary(BinaryPathControl control)
	{
		if (string.IsNullOrWhiteSpace(_outputPath) || string.IsNullOrWhiteSpace(control.BinaryPath) || !Directory.Exists(_outputPath) || !File.Exists(control.BinaryPath))
			return;

		await Task.Run(() =>
		{
			try
			{
				string? error = BinaryHandler.ExtractBinary(control.BinaryPath, _outputPath, control.Progress);
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

	private void CreateModFile()
	{
		if (string.IsNullOrWhiteSpace(_modFilePath) || string.IsNullOrWhiteSpace(_outputPath) || !Directory.Exists(_outputPath))
			return;

		Dictionary<string, float> loudnessValues = new();
		string[] loudnessPaths = Directory.GetFiles(_outputPath, "*.ini", SearchOption.AllDirectories);
		if (loudnessPaths.Length > 0)
		{
			foreach (string line in File.ReadAllLines(loudnessPaths[0]))
			{
				if (!LoudnessUtils.TryReadLoudnessLine(line, out string? assetName, out float loudness))
					continue;

				loudnessValues.Add(assetName!, loudness);
			}
		}

		List<UserAsset> modFile = new();
		foreach (string filePath in Directory.GetFiles(_outputPath, "*.*", SearchOption.AllDirectories))
		{
			string extension = Path.GetExtension(filePath);
			AssetType? assetType = extension.GetAssetType();
			if (!assetType.HasValue)
				continue;

			string assetName;
			switch (extension)
			{
				case ".glsl":
					// Skip fragment shader file and let vertex handle the creation of ShaderUserAsset.
					if (Path.GetFileNameWithoutExtension(filePath).EndsWith("_fragment"))
						continue;

					assetName = Path.GetFileNameWithoutExtension(filePath).TrimEnd("_vertex");
					modFile.Add(new ShaderUserAsset(assetName, filePath, filePath.Replace("_vertex", "_fragment")));
					break;
				case ".wav":
					assetName = Path.GetFileNameWithoutExtension(filePath);
					float loudness = loudnessValues.ContainsKey(assetName) ? loudnessValues[assetName] : 1;
					modFile.Add(new AudioUserAsset(assetName, filePath, loudness));
					break;
				default:
					assetName = Path.GetFileNameWithoutExtension(filePath);
					modFile.Add(new UserAsset(assetType.Value, assetName, filePath));
					break;
			}
		}

		JsonFileUtils.SerializeToFile(_modFilePath, modFile, true);
	}
}
