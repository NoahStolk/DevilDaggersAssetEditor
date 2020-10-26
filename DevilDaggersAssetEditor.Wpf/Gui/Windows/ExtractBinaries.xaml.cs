using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using Microsoft.Win32;
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
		private string _audioPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Audio.GetSubfolderName(), BinaryFileType.Audio.ToString().ToLower(CultureInfo.InvariantCulture));
		private string _corePath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Core.GetSubfolderName(), BinaryFileType.Core.ToString().ToLower(CultureInfo.InvariantCulture));
		private string _ddPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Dd.GetSubfolderName(), BinaryFileType.Dd.ToString().ToLower(CultureInfo.InvariantCulture));
		private string _particlePath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Particle.GetSubfolderName(), BinaryFileType.Particle.ToString().ToLower(CultureInfo.InvariantCulture));
		private string? _outputPath;

		private readonly ProgressWrapper _audioProgress;
		private readonly ProgressWrapper _coreProgress;
		private readonly ProgressWrapper _ddProgress;
		private readonly ProgressWrapper _particleProgress;

		public ExtractBinariesWindow()
		{
			InitializeComponent();
			UpdateGui();

			_audioProgress = new ProgressWrapper(
				new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => ProgressDescriptionAudio.Text = value)),
				new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => ProgressBarAudio.Value = value)));
			_coreProgress = new ProgressWrapper(
				new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => ProgressDescriptionCore.Text = value)),
				new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => ProgressBarCore.Value = value)));
			_ddProgress = new ProgressWrapper(
				new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => ProgressDescriptionDd.Text = value)),
				new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => ProgressBarDd.Value = value)));
			_particleProgress = new ProgressWrapper(
				new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => ProgressDescriptionParticle.Text = value)),
				new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => ProgressBarParticle.Value = value)));
		}

		private void UpdateGui()
		{
			TextBoxAudio.Text = _audioPath;
			TextBoxCore.Text = _corePath;
			TextBoxDd.Text = _ddPath;
			TextBoxParticle.Text = _particlePath;
			TextBoxOutput.Text = _outputPath;
		}

		private void BrowseAudioButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Audio, ref _audioPath);

		private void BrowseCoreButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Core, ref _corePath);

		private void BrowseDdButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Dd, ref _ddPath);

		private void BrowseParticleButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Particle, ref _particlePath);

		private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
			folderDialog.OpenAssetsRootFolder();

			if (folderDialog.ShowDialog() == true)
			{
				_outputPath = folderDialog.SelectedPath;
				UpdateGui();
			}
		}

		private void TextBoxAudio_TextChanged(object sender, TextChangedEventArgs e)
			=> _audioPath = TextBoxAudio.Text;

		private void TextBoxCore_TextChanged(object sender, TextChangedEventArgs e)
			=> _corePath = TextBoxCore.Text;

		private void TextBoxDd_TextChanged(object sender, TextChangedEventArgs e)
			=> _ddPath = TextBoxDd.Text;

		private void TextBoxParticle_TextChanged(object sender, TextChangedEventArgs e)
			=> _particlePath = TextBoxParticle.Text;

		private void TextBoxOutput_TextChanged(object sender, TextChangedEventArgs e)
			=> _outputPath = TextBoxOutput.Text;

		private void SetPath(BinaryFileType binaryFileType, ref string path)
		{
			if (TrySetPath(binaryFileType, out string selectedPath))
			{
				path = selectedPath;
				UpdateGui();
			}
		}

		private static bool TrySetPath(BinaryFileType binaryFileType, out string selectedPath)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			string initDir = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, binaryFileType.GetSubfolderName());
			if (UserHandler.Instance.Settings.EnableDevilDaggersRootFolder && Directory.Exists(initDir))
				openDialog.InitialDirectory = initDir;

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = openDialog.FileName;
			return true;
		}

		private async void ExtractBinaries_Click(object sender, RoutedEventArgs e)
		{
			await Task.WhenAll(new List<Task>
			{
				ExtractBinary(BinaryFileType.Audio, _audioPath, _outputPath, _audioProgress, ProgressBarAudio, ProgressDescriptionAudio),
				ExtractBinary(BinaryFileType.Core, _corePath, _outputPath, _coreProgress, ProgressBarCore, ProgressDescriptionCore),
				ExtractBinary(BinaryFileType.Dd, _ddPath, _outputPath, _ddProgress, ProgressBarDd, ProgressDescriptionDd),
				ExtractBinary(BinaryFileType.Particle, _particlePath, _outputPath, _particleProgress, ProgressBarParticle, ProgressDescriptionParticle),
			});

			if (!string.IsNullOrWhiteSpace(_outputPath) && Directory.Exists(_outputPath))
			{
				if (UserHandler.Instance.Settings.CreateModFileWhenExtracting)
					ModFileUtils.CreateModFileFromPath(_outputPath);

				if (UserHandler.Instance.Settings.OpenModFolderAfterExtracting)
					Process.Start($@"{Environment.GetEnvironmentVariable("WINDIR")}\explorer.exe", _outputPath);
			}
		}

		private static async Task ExtractBinary(BinaryFileType binaryFileType, string? inputPath, string? outputPath, ProgressWrapper progress, ProgressBar progressBar, TextBlock progressDescription)
		{
			if (string.IsNullOrWhiteSpace(outputPath) || string.IsNullOrWhiteSpace(inputPath) || !Directory.Exists(outputPath) || !File.Exists(inputPath))
				return;

			string binaryFileTypeName = binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture);

			await Task.Run(() =>
			{
				try
				{
					AbstractBinaryFileHandler fileHandler = binaryFileType switch
					{
						BinaryFileType.Particle => new ParticleFileHandler(),
						_ => new ResourceFileHandler(binaryFileType),
					};

					fileHandler.ExtractBinary(inputPath, outputPath, binaryFileType, progress);
					App.Instance.Dispatcher.Invoke(() => progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError($"Extracting binary '{binaryFileTypeName}' did not complete successfully", $"An error occurred during the execution of \"{progressDescription.Text}\".", ex);
						progress.Report("Execution did not complete successfully.");
					});
				}
			});
		}
	}
}