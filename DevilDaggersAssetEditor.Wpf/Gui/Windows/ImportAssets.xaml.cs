using DevilDaggersAssetEditor.Mods;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersCore.Wpf.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows;

public partial class ImportAssetsWindow : Window
{
	private static readonly UserCache _cache = UserHandler.Instance.Cache;

	private readonly ImportDirectoryControl _audioAudioControl = new(
		"'audio/Audio' import directory",
		AssetType.Audio,
		App.Instance.MainWindow!.AudioAudioAssetTabControl,
		_cache.ImportAudioAudio,
		_cache.ImportAudioAudioAllDirectories,
		(b) => _cache.ImportAudioAudio = b,
		(b) => _cache.ImportAudioAudioAllDirectories = b);
	private readonly ImportDirectoryControl _coreShadersControl = new(
		"'core/Shaders' import directory",
		AssetType.Shader,
		App.Instance.MainWindow!.CoreShadersAssetTabControl,
		_cache.ImportCoreShaders,
		_cache.ImportCoreShadersAllDirectories,
		(b) => _cache.ImportCoreShaders = b,
		(b) => _cache.ImportCoreShadersAllDirectories = b);
	private readonly ImportDirectoryControl _ddModelBindingsControl = new(
		"'dd/Model Bindings' import directory",
		AssetType.ModelBinding,
		App.Instance.MainWindow!.DdModelBindingsAssetTabControl,
		_cache.ImportDdModelBindings,
		_cache.ImportDdModelBindingsAllDirectories,
		(b) => _cache.ImportDdModelBindings = b,
		(b) => _cache.ImportDdModelBindingsAllDirectories = b);
	private readonly ImportDirectoryControl _ddModelsControl = new(
		"'dd/Models' import directory",
		AssetType.Model,
		App.Instance.MainWindow!.DdModelsAssetTabControl,
		_cache.ImportDdModels,
		_cache.ImportDdModelsAllDirectories,
		(b) => _cache.ImportDdModels = b,
		(b) => _cache.ImportDdModelsAllDirectories = b);
	private readonly ImportDirectoryControl _ddShadersControl = new(
		"'dd/Shaders' import directory",
		AssetType.Shader,
		App.Instance.MainWindow!.DdShadersAssetTabControl,
		_cache.ImportDdShaders,
		_cache.ImportDdShadersAllDirectories,
		(b) => _cache.ImportDdShaders = b,
		(b) => _cache.ImportDdShadersAllDirectories = b);
	private readonly ImportDirectoryControl _ddTexturesControl = new(
		"'dd/Textures' import directory",
		AssetType.Texture,
		App.Instance.MainWindow!.DdTexturesAssetTabControl,
		_cache.ImportDdTextures,
		_cache.ImportDdTexturesAllDirectories,
		(b) => _cache.ImportDdTextures = b,
		(b) => _cache.ImportDdTexturesAllDirectories = b);

	private readonly List<ImportDirectoryControl> _controls = new();

	public ImportAssetsWindow()
	{
		InitializeComponent();

		_controls.Add(_audioAudioControl);
		_controls.Add(_coreShadersControl);
		_controls.Add(_ddModelBindingsControl);
		_controls.Add(_ddModelsControl);
		_controls.Add(_ddShadersControl);
		_controls.Add(_ddTexturesControl);

		for (int i = 0; i < _controls.Count; i++)
			Main.Children.Insert(i, _controls[i]);
	}

	private void ImportAssetsButton_Click(object sender, RoutedEventArgs e)
	{
		foreach (ImportDirectoryControl control in _controls.Where(idc => idc.CheckBoxEnable.IsChecked()))
			control.AssetTabControl.ImportFolder(control.Directory, control.AssetType, control.CheckBoxAllDirectories.IsChecked() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

		Close();
	}
}
