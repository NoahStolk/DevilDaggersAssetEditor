using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ImportAssetsWindow : Window
	{
		private readonly ImportDirectoryControl _audioAudioControl = new ImportDirectoryControl("'audio/Audio' import directory");
		private readonly ImportDirectoryControl _coreShadersControl = new ImportDirectoryControl("'core/Shaders' import directory");
		private readonly ImportDirectoryControl _ddModelBindingsControl = new ImportDirectoryControl("'dd/Model Bindings' import directory");
		private readonly ImportDirectoryControl _ddModelsControl = new ImportDirectoryControl("'dd/Models' import directory");
		private readonly ImportDirectoryControl _ddShadersControl = new ImportDirectoryControl("'dd/Shaders' import directory");
		private readonly ImportDirectoryControl _ddTexturesControl = new ImportDirectoryControl("'dd/Textures' import directory");
		private readonly ImportDirectoryControl _particleParticlesControl = new ImportDirectoryControl("'particle/Particles' import directory");

		public ImportAssetsWindow()
		{
			InitializeComponent();

			Main.Children.Insert(0, _audioAudioControl);
			Main.Children.Insert(1, _coreShadersControl);
			Main.Children.Insert(2, _ddModelBindingsControl);
			Main.Children.Insert(3, _ddModelsControl);
			Main.Children.Insert(4, _ddShadersControl);
			Main.Children.Insert(5, _ddTexturesControl);
			Main.Children.Insert(6, _particleParticlesControl);
		}

		private void ImportAssetsButton_Click(object sender, RoutedEventArgs e)
		{
			App.Instance.MainWindow!.AudioAudioAssetTabControl.ImportFolder(_audioAudioControl.Directory, AssetType.Audio);
			App.Instance.MainWindow!.CoreShadersAssetTabControl.ImportFolder(_coreShadersControl.Directory, AssetType.Shader);
			App.Instance.MainWindow!.DdModelBindingsAssetTabControl.ImportFolder(_ddModelBindingsControl.Directory, AssetType.ModelBinding);
			App.Instance.MainWindow!.DdModelsAssetTabControl.ImportFolder(_ddModelsControl.Directory, AssetType.Model);
			App.Instance.MainWindow!.DdShadersAssetTabControl.ImportFolder(_ddShadersControl.Directory, AssetType.Shader);
			App.Instance.MainWindow!.DdTexturesAssetTabControl.ImportFolder(_ddTexturesControl.Directory, AssetType.Texture);
			App.Instance.MainWindow!.ParticleParticlesAssetTabControl.ImportFolder(_particleParticlesControl.Directory, AssetType.Particle);

			Close();
		}
	}
}