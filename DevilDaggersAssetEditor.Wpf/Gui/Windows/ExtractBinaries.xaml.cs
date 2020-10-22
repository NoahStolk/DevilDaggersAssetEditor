using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ExtractBinariesWindow : Window
	{
		public ExtractBinariesWindow()
		{
			InitializeComponent();

			Data.DataContext = this;
		}

		public string? AudioPath { get; private set; }
		public string? CorePath { get; private set; }
		public string? DdPath { get; private set; }
		public string? ParticlePath { get; private set; }
		public string? OutputPath { get; private set; }

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

		private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
			if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.ModsRootFolder))
				folderDialog.SelectedPath = $"{UserHandler.Instance.Settings.ModsRootFolder}\\";

			if (folderDialog.ShowDialog() == true)
				OutputPath = folderDialog.SelectedPath;
		}

		private async void ExtractBinaries_Click(object sender, RoutedEventArgs e)
		{
			await ExtractBinary(BinaryFileType.Audio, AudioPath, OutputPath);
			await ExtractBinary(BinaryFileType.Core, CorePath, OutputPath);
			await ExtractBinary(BinaryFileType.Dd, DdPath, OutputPath);
			await ExtractBinary(BinaryFileType.Particle, ParticlePath, OutputPath);
		}

		private static async Task ExtractBinary(BinaryFileType binaryFileType, string? inputPath, string? outputPath)
		{
			if (outputPath == null || inputPath == null || !Directory.Exists(outputPath) || !File.Exists(inputPath))
				return;

			ProgressWindow progressWindow = new ProgressWindow($"Extracting '{binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}'...");
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

					fileHandler.ExtractBinary(
						inputPath,
						outputPath,
						binaryFileType,
						new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressBar.Value = value)),
						new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressDescription.Text = value)));
					App.Instance.Dispatcher.Invoke(() => progressWindow.Finish());
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Extracting binary did not complete successfully", $"An error occurred during the execution of \"{progressWindow.ProgressDescription.Text}\".", ex);
						progressWindow.Error();
					});
				}
			});
		}
	}
}