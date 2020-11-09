using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
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
		private readonly BinaryPathControl _audioControl = new BinaryPathControl("'audio' binary path", BinaryFileType.Audio, AssetType.Audio);
		private readonly BinaryPathControl _coreControl = new BinaryPathControl("'core' binary path", BinaryFileType.Core, AssetType.Shader);
		private readonly BinaryPathControl _ddControl = new BinaryPathControl("'dd' binary path", BinaryFileType.Dd, AssetType.Texture);
		private readonly BinaryPathControl _particleControl = new BinaryPathControl("'particle' binary path", BinaryFileType.Particle, AssetType.Particle);

		public MakeBinariesWindow()
		{
			InitializeComponent();

			Main.Children.Insert(0, _audioControl);
			Main.Children.Insert(1, _coreControl);
			Main.Children.Insert(2, _ddControl);
			Main.Children.Insert(3, _particleControl);
		}

		private async void MakeBinaries_Click(object sender, RoutedEventArgs e)
		{
			ButtonMakeBinaries.IsEnabled = false;

			await Task.WhenAll(new List<Task>
			{
				MakeBinary(BinaryFileType.Audio, _audioControl.BinaryPath, _audioControl.Progress),
				MakeBinary(BinaryFileType.Core, _coreControl.BinaryPath, _coreControl.Progress),
				MakeBinary(BinaryFileType.Dd, _ddControl.BinaryPath, _ddControl.Progress),
				MakeBinary(BinaryFileType.Particle, _particleControl.BinaryPath, _particleControl.Progress),
			});

			ButtonMakeBinaries.IsEnabled = true;
		}

		private static async Task MakeBinary(BinaryFileType binaryFileType, string? outputPath, ProgressWrapper progress)
		{
			if (string.IsNullOrWhiteSpace(outputPath) || !File.Exists(outputPath))
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