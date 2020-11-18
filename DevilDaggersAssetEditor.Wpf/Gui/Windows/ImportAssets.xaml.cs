using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersCore.Wpf.Extensions;
using System.Collections.Generic;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ImportAssetsWindow : Window
	{
		private readonly ImportDirectoryControl _audioAudioControl = new ImportDirectoryControl("'audio/Audio' import directory", AssetType.Audio, App.Instance.MainWindow!.AudioAudioAssetTabControl);
		private readonly ImportDirectoryControl _coreShadersControl = new ImportDirectoryControl("'core/Shaders' import directory", AssetType.Shader, App.Instance.MainWindow!.CoreShadersAssetTabControl);
		private readonly ImportDirectoryControl _ddModelBindingsControl = new ImportDirectoryControl("'dd/Model Bindings' import directory", AssetType.ModelBinding, App.Instance.MainWindow!.DdModelBindingsAssetTabControl);
		private readonly ImportDirectoryControl _ddModelsControl = new ImportDirectoryControl("'dd/Models' import directory", AssetType.Model, App.Instance.MainWindow!.DdModelsAssetTabControl);
		private readonly ImportDirectoryControl _ddShadersControl = new ImportDirectoryControl("'dd/Shaders' import directory", AssetType.Shader, App.Instance.MainWindow!.DdShadersAssetTabControl);
		private readonly ImportDirectoryControl _ddTexturesControl = new ImportDirectoryControl("'dd/Textures' import directory", AssetType.Texture, App.Instance.MainWindow!.DdTexturesAssetTabControl);
		private readonly ImportDirectoryControl _particleParticlesControl = new ImportDirectoryControl("'particle/Particles' import directory", AssetType.Particle, App.Instance.MainWindow!.ParticleParticlesAssetTabControl);

		private readonly List<ImportDirectoryControl> _controls = new List<ImportDirectoryControl>();

		public ImportAssetsWindow()
		{
			InitializeComponent();

			_controls.Add(_audioAudioControl);
			_controls.Add(_coreShadersControl);
			_controls.Add(_ddModelBindingsControl);
			_controls.Add(_ddModelsControl);
			_controls.Add(_ddShadersControl);
			_controls.Add(_ddTexturesControl);
			_controls.Add(_particleParticlesControl);

			for (int i = 0; i < _controls.Count; i++)
				Main.Children.Insert(i, _controls[i]);
		}

		private void ImportAssetsButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (ImportDirectoryControl control in _controls)
			{
				if (control.CheckBoxEnable.IsChecked())
					control.AssetTabControl.ImportFolder(control.Directory, control.AssetType);
			}

			Close();
		}
	}
}