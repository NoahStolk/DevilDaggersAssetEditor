using DevilDaggersAssetEditor.User;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ImportAssetsWindow : Window
	{
		public ImportAssetsWindow()
		{
			InitializeComponent();
		}

		public string? AudioAudioPath { get; private set; }
		public string? CoreShadersPath { get; private set; }
		public string? DdModelBindingsPath { get; private set; }
		public string? DdModelsPath { get; private set; }
		public string? DdShadersPath { get; private set; }
		public string? DdTexturesPath { get; private set; }
		public string? ParticleParticlesPath { get; private set; }

		private void BrowseAudioAudioButton_Click(object sender, RoutedEventArgs e)
			=> AudioAudioPath = SetPath();

		private void BrowseCoreShadersButton_Click(object sender, RoutedEventArgs e)
			=> CoreShadersPath = SetPath();

		private void BrowseDdModelBindingsButton_Click(object sender, RoutedEventArgs e)
			=> DdModelBindingsPath = SetPath();

		private void BrowseDdModelsButton_Click(object sender, RoutedEventArgs e)
			=> DdModelsPath = SetPath();

		private void BrowseDdShadersButton_Click(object sender, RoutedEventArgs e)
			=> DdShadersPath = SetPath();

		private void BrowseDdTexturesButton_Click(object sender, RoutedEventArgs e)
			=> DdTexturesPath = SetPath();

		private void BrowseParticleParticlesButton_Click(object sender, RoutedEventArgs e)
			=> ParticleParticlesPath = SetPath();

		private static string? SetPath()
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			if (UserHandler.Instance.Settings.EnableAssetsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
				dialog.SelectedPath = $"{UserHandler.Instance.Settings.AssetsRootFolder}\\";

			if (dialog.ShowDialog() != true)
				return null;

			return dialog.SelectedPath;
		}

		private void ImportAssetsButton_Click(object sender, RoutedEventArgs e)
		{
			App.Instance.MainWindow!.AudioAudioAssetTabControl.ImportFolder(AudioAudioPath);
			App.Instance.MainWindow!.CoreShadersAssetTabControl.ImportFolder(CoreShadersPath);
			App.Instance.MainWindow!.DdModelBindingsAssetTabControl.ImportFolder(DdModelBindingsPath);
			App.Instance.MainWindow!.DdModelsAssetTabControl.ImportFolder(DdModelsPath);
			App.Instance.MainWindow!.DdShadersAssetTabControl.ImportFolder(DdShadersPath);
			App.Instance.MainWindow!.DdTexturesAssetTabControl.ImportFolder(DdTexturesPath);
			App.Instance.MainWindow!.ParticleParticlesAssetTabControl.ImportFolder(ParticleParticlesPath);
		}
	}
}