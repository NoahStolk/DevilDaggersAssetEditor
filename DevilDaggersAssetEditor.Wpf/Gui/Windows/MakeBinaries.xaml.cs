using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class MakeBinariesWindow : Window
	{
		private string _audioPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Audio.GetSubfolderName(), BinaryFileType.Audio.ToString().ToLower(CultureInfo.InvariantCulture));
		private string _corePath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Core.GetSubfolderName(), BinaryFileType.Core.ToString().ToLower(CultureInfo.InvariantCulture));
		private string _ddPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Dd.GetSubfolderName(), BinaryFileType.Dd.ToString().ToLower(CultureInfo.InvariantCulture));
		private string _particlePath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.Particle.GetSubfolderName(), BinaryFileType.Particle.ToString().ToLower(CultureInfo.InvariantCulture));

		private readonly ProgressWrapper _audioProgress;
		private readonly ProgressWrapper _coreProgress;
		private readonly ProgressWrapper _ddProgress;
		private readonly ProgressWrapper _particleProgress;

		public MakeBinariesWindow()
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
		}

		private void BrowseAudioButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Audio, ref _audioPath);

		private void BrowseCoreButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Core, ref _corePath);

		private void BrowseDdButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Dd, ref _ddPath);

		private void BrowseParticleButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(BinaryFileType.Particle, ref _particlePath);

		private void TextBoxAudio_TextChanged(object sender, TextChangedEventArgs e)
			=> _audioPath = TextBoxAudio.Text;

		private void TextBoxCore_TextChanged(object sender, TextChangedEventArgs e)
			=> _corePath = TextBoxCore.Text;

		private void TextBoxDd_TextChanged(object sender, TextChangedEventArgs e)
			=> _ddPath = TextBoxDd.Text;

		private void TextBoxParticle_TextChanged(object sender, TextChangedEventArgs e)
			=> _particlePath = TextBoxParticle.Text;

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
			openDialog.OpenDirectory(UserHandler.Instance.Settings.EnableDevilDaggersRootFolder, Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, binaryFileType.GetSubfolderName()));

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = openDialog.FileName;
			return true;
		}

		private async void MakeBinaries_Click(object sender, RoutedEventArgs e)
		{
			await Task.WhenAll(new List<Task>
			{
				MakeBinary(BinaryFileType.Audio, _audioPath, _audioProgress),
				MakeBinary(BinaryFileType.Core, _corePath, _coreProgress),
				MakeBinary(BinaryFileType.Dd, _ddPath, _ddProgress),
				MakeBinary(BinaryFileType.Particle, _particlePath, _particleProgress),
			});
		}

		private static async Task MakeBinary(BinaryFileType binaryFileType, string? outputPath, ProgressWrapper progress)
		{
			if (string.IsNullOrWhiteSpace(outputPath) || !File.Exists(outputPath))
				return;

			await Task.Run(() =>
			{
				try
				{
					AbstractBinaryFileHandler fileHandler = binaryFileType switch
					{
						BinaryFileType.Particle => new ParticleFileHandler(),
						_ => new ResourceFileHandler(binaryFileType),
					};

					List<AssetTabControl> assetTabControls = new List<AssetTabControl>();
					switch (binaryFileType)
					{
						case BinaryFileType.Audio:
							assetTabControls.Add(App.Instance.MainWindow!.AudioAudioAssetTabControl);
							break;
						case BinaryFileType.Core:
							assetTabControls.Add(App.Instance.MainWindow!.CoreShadersAssetTabControl);
							break;
						case BinaryFileType.Dd:
							assetTabControls.Add(App.Instance.MainWindow!.DdModelBindingsAssetTabControl);
							assetTabControls.Add(App.Instance.MainWindow!.DdModelsAssetTabControl);
							assetTabControls.Add(App.Instance.MainWindow!.DdShadersAssetTabControl);
							assetTabControls.Add(App.Instance.MainWindow!.DdTexturesAssetTabControl);
							break;
						case BinaryFileType.Particle:
							assetTabControls.Add(App.Instance.MainWindow!.ParticleParticlesAssetTabControl);
							break;
					}

					fileHandler.MakeBinary(assetTabControls.SelectMany(atc => atc.GetAssets()).ToList(), outputPath, progress);

					App.Instance.Dispatcher.Invoke(() => progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Making binary did not complete successfully", $"An error occurred while making '{binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' binary.", ex);
						progress.Report("Execution did not complete successfully.");
					});
				}
			});
		}
	}
}