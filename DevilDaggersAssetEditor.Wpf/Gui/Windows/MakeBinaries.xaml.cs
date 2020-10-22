using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class MakeBinariesWindow : Window
	{
		public MakeBinariesWindow()
		{
			InitializeComponent();

			Data.DataContext = this;
		}

		public string? AudioPath { get; private set; }
		public string? CorePath { get; private set; }
		public string? DdPath { get; private set; }
		public string? ParticlePath { get; private set; }

		private void BrowseAudioButton_Click(object sender, RoutedEventArgs e)
			=> AudioPath = SetPath(BinaryFileType.Audio);

		private void BrowseCoreButton_Click(object sender, RoutedEventArgs e)
			=> CorePath = SetPath(BinaryFileType.Core);

		private void BrowseDdButton_Click(object sender, RoutedEventArgs e)
			=> DdPath = SetPath(BinaryFileType.Dd);

		private void BrowseParticleButton_Click(object sender, RoutedEventArgs e)
			=> ParticlePath = SetPath(BinaryFileType.Particle);

		private static string? SetPath(BinaryFileType binaryFileType)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			string initDir = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, binaryFileType.GetSubfolderName());
			if (UserHandler.Instance.Settings.EnableDevilDaggersRootFolder && Directory.Exists(initDir))
				openDialog.InitialDirectory = initDir;

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return null;

			return openDialog.FileName;
		}

		private async void MakeBinaries_Click(object sender, RoutedEventArgs e)
		{
			await MakeBinary(BinaryFileType.Audio, AudioPath);
			await MakeBinary(BinaryFileType.Core, CorePath);
			await MakeBinary(BinaryFileType.Dd, DdPath);
			await MakeBinary(BinaryFileType.Particle, ParticlePath);
		}

		private static async Task MakeBinary(BinaryFileType binaryFileType, string? outputPath)
		{
			if (outputPath == null || !File.Exists(outputPath))
				return;

			ProgressWindow progressWindow = new ProgressWindow($"Turning files into '{binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' binary...");
			progressWindow.Show();
			await Task.Run(() =>
			{
				try
				{
					AbstractBinaryFileHandler fileHandler = binaryFileType switch
					{
						BinaryFileType.Particle => new ParticleFileHandler(),
						_ => new ResourceFileHandler(binaryFileType),
					};

					List<AssetTabControl> atcs = new List<AssetTabControl>();
					switch (binaryFileType)
					{
						case BinaryFileType.Particle:
							atcs.Add(App.Instance.MainWindow!.ParticleParticlesAssetTabControl);
							break;
						case BinaryFileType.Audio:
							atcs.Add(App.Instance.MainWindow!.AudioAudioAssetTabControl);
							break;
						case BinaryFileType.Core:
							atcs.Add(App.Instance.MainWindow!.CoreShadersAssetTabControl);
							break;
						case BinaryFileType.Dd:
							atcs.Add(App.Instance.MainWindow!.DdModelBindingsAssetTabControl);
							atcs.Add(App.Instance.MainWindow!.DdModelsAssetTabControl);
							atcs.Add(App.Instance.MainWindow!.DdShadersAssetTabControl);
							atcs.Add(App.Instance.MainWindow!.DdTexturesAssetTabControl);
							break;
					}

					fileHandler.MakeBinary(
						allAssets: atcs.SelectMany(atc => atc.GetAssets()).ToList(),
						outputPath: outputPath,
						progress: new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressBar.Value = value)),
						progressDescription: new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressDescription.Text = value)));

					App.Instance.Dispatcher.Invoke(() => progressWindow.Finish());
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Making binary did not complete successfully", $"An error occurred during the execution of \"{progressWindow.ProgressDescription.Text}\".", ex);
						progressWindow.Error();
					});
				}
			});
		}
	}
}