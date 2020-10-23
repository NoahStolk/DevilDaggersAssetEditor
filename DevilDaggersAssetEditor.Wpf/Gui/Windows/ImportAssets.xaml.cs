using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ImportAssetsWindow : Window
	{
		private string _audioAudioPath = UserHandler.Instance.Settings.AssetsRootFolder;
		private string _coreShadersPath = UserHandler.Instance.Settings.AssetsRootFolder;
		private string _ddModelBindingsPath = UserHandler.Instance.Settings.AssetsRootFolder;
		private string _ddModelsPath = UserHandler.Instance.Settings.AssetsRootFolder;
		private string _ddShadersPath = UserHandler.Instance.Settings.AssetsRootFolder;
		private string _ddTexturesPath = UserHandler.Instance.Settings.AssetsRootFolder;
		private string _particleParticlesPath = UserHandler.Instance.Settings.AssetsRootFolder;

		public ImportAssetsWindow()
		{
			InitializeComponent();
			UpdateGui();
		}

		private void UpdateGui()
		{
			TextBoxAudioAudio.Text = _audioAudioPath;
			TextBoxCoreShaders.Text = _coreShadersPath;
			TextBoxDdModelBindings.Text = _ddModelBindingsPath;
			TextBoxDdModels.Text = _ddModelsPath;
			TextBoxDdShaders.Text = _ddShadersPath;
			TextBoxDdTextures.Text = _ddTexturesPath;
			TextBoxParticleParticles.Text = _particleParticlesPath;
		}

		private void BrowseAudioAudioButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _audioAudioPath);

		private void BrowseCoreShadersButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _coreShadersPath);

		private void BrowseDdModelBindingsButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _ddModelBindingsPath);

		private void BrowseDdModelsButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _ddModelsPath);

		private void BrowseDdShadersButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _ddShadersPath);

		private void BrowseDdTexturesButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _ddTexturesPath);

		private void BrowseParticleParticlesButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _particleParticlesPath);

		private void TextBoxAudioAudio_TextChanged(object sender, TextChangedEventArgs e)
			=> _audioAudioPath = TextBoxAudioAudio.Text;

		private void TextBoxCoreShaders_TextChanged(object sender, TextChangedEventArgs e)
			=> _coreShadersPath = TextBoxCoreShaders.Text;

		private void TextBoxDdModelBindings_TextChanged(object sender, TextChangedEventArgs e)
			=> _ddModelBindingsPath = TextBoxDdModelBindings.Text;

		private void TextBoxDdModels_TextChanged(object sender, TextChangedEventArgs e)
			=> _ddModelsPath = TextBoxDdModels.Text;

		private void TextBoxDdShaders_TextChanged(object sender, TextChangedEventArgs e)
			=> _ddShadersPath = TextBoxDdShaders.Text;

		private void TextBoxDdTextures_TextChanged(object sender, TextChangedEventArgs e)
			=> _ddTexturesPath = TextBoxDdTextures.Text;

		private void TextBoxParticleParticles_TextChanged(object sender, TextChangedEventArgs e)
			=> _particleParticlesPath = TextBoxParticleParticles.Text;

		private void SetPath(ref string path)
		{
			if (TrySetPath(out string selectedPath))
			{
				path = selectedPath;
				UpdateGui();
			}
		}

		private static bool TrySetPath(out string selectedPath)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			if (UserHandler.Instance.Settings.EnableAssetsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
				dialog.SelectedPath = $"{UserHandler.Instance.Settings.AssetsRootFolder}\\";

			if (dialog.ShowDialog() != true)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = dialog.SelectedPath;
			return true;
		}

		private void ImportAssetsButton_Click(object sender, RoutedEventArgs e)
		{
			App.Instance.MainWindow!.AudioAudioAssetTabControl.ImportFolder(_audioAudioPath, AssetType.Audio);
			App.Instance.MainWindow!.CoreShadersAssetTabControl.ImportFolder(_coreShadersPath, AssetType.Shader);
			App.Instance.MainWindow!.DdModelBindingsAssetTabControl.ImportFolder(_ddModelBindingsPath, AssetType.ModelBinding);
			App.Instance.MainWindow!.DdModelsAssetTabControl.ImportFolder(_ddModelsPath, AssetType.Model);
			App.Instance.MainWindow!.DdShadersAssetTabControl.ImportFolder(_ddShadersPath, AssetType.Shader);
			App.Instance.MainWindow!.DdTexturesAssetTabControl.ImportFolder(_ddTexturesPath, AssetType.Texture);
			App.Instance.MainWindow!.ParticleParticlesAssetTabControl.ImportFolder(_particleParticlesPath, AssetType.Particle);
		}
	}
}