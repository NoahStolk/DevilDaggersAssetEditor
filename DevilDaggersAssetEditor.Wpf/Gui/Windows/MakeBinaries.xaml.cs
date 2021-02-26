using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class MakeBinariesWindow : Window
	{
		private readonly BinaryNameControl _audioControl;
		private readonly BinaryNameControl _coreControl;
		private readonly BinaryNameControl _ddControl;
		private readonly BinaryNameControl _particleControl;

		private readonly List<BinaryNameControl> _controls = new();

		public MakeBinariesWindow()
		{
			InitializeComponent();

			_audioControl = new(this, BinaryFileType.Audio, AssetType.Audio, App.Instance.MainWindow!.HasAnyAudioFiles());
			_coreControl = new(this, BinaryFileType.Core, AssetType.Shader, App.Instance.MainWindow!.HasAnyCoreFiles());
			_ddControl = new(this, BinaryFileType.Dd, AssetType.Texture, App.Instance.MainWindow!.HasAnyDdFiles());
			_particleControl = new(this, BinaryFileType.Particle, AssetType.Particle, App.Instance.MainWindow!.HasAnyParticleFiles());

			_controls.Add(_audioControl);
			_controls.Add(_coreControl);
			_controls.Add(_ddControl);
			_controls.Add(_particleControl);

			for (int i = 0; i < _controls.Count; i++)
				Main.Children.Insert(i + 1, _controls[i]);

			UpdateButtonMakeBinaries();
		}

		private async void MakeBinaries_Click(object sender, RoutedEventArgs e)
		{
			ButtonMakeBinaries.IsEnabled = false;

			await Task.WhenAll(_controls.Where(c => c.CheckBoxEnable.IsChecked()).Select(c => MakeBinary(c)));

			ButtonMakeBinaries.IsEnabled = true;
		}

		private static async Task MakeBinary(BinaryNameControl control)
		{
			if (string.IsNullOrWhiteSpace(control.BinaryName) || string.IsNullOrWhiteSpace(control.OutputPath))
				return;

			await Task.Run(() =>
			{
				try
				{
					IBinaryFileHandler fileHandler = control.BinaryFileType switch
					{
						BinaryFileType.Particle => new ParticleFileHandler(),
						_ => new ResourceFileHandler(control.BinaryFileType),
					};

					List<AssetTabControl> assetTabControls = new();
					switch (control.BinaryFileType)
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

					fileHandler.MakeBinary(assetTabControls.SelectMany(atc => atc.GetAssets()).ToList(), control.OutputPath, control.Progress);

					App.Instance.Dispatcher.Invoke(() => control.Progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Making binary did not complete successfully", $"An error occurred while making '{control.BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' binary.", ex);
						control.Progress.Report("Execution did not complete successfully.");
					});
				}
			});
		}

		private void TextBoxModName_TextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (BinaryNameControl control in _controls)
			{
				control.UpdateName(TextBoxModName.Text);
				control.UpdateGui();
			}

			UpdateButtonMakeBinaries();
		}

		public void UpdateButtonMakeBinaries()
		{
			ButtonMakeBinaries.IsEnabled = _controls.Any(c => c.CheckBoxEnable.IsChecked() && !string.IsNullOrWhiteSpace(c.OutputPath));
		}
	}
}
