using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersAssetEditor.Wpf.ModFiles;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		private readonly List<BinaryNameControl> _controls = new();

		public MakeBinariesWindow()
		{
			InitializeComponent();

			TextBoxModName.Text = UserHandler.Instance.Cache.MakeBinaryName;

			bool hasAudioAssets = ModFileHandler.Instance.ModFile.Any(a =>
				a.AssetType == AssetType.Audio && // Must be audio.
				a.EditorPath != GuiUtils.FileNotFound); // Must have file.

			bool hasCoreAssets = ModFileHandler.Instance.ModFile.Any(a =>
				a is ShaderUserAsset shaderAsset && // Must be shader.
				a.EditorPath != GuiUtils.FileNotFound && // Must have vertex file.
				shaderAsset.EditorPathFragmentShader != GuiUtils.FileNotFound && // Must have fragment file.
				AssetContainer.Instance.CoreShadersAssets.Any(sa => sa.AssetName == a.AssetName)); // Must be present in core.

			bool hasDdAssets = ModFileHandler.Instance.ModFile.Any(a =>
				a.AssetType != AssetType.Audio && // Must not be audio.
				a.EditorPath != GuiUtils.FileNotFound && // Must have (vertex) file.
					(a is not ShaderUserAsset shaderAsset || // Must either not be a shader.
					shaderAsset.EditorPathFragmentShader != GuiUtils.FileNotFound && !AssetContainer.Instance.CoreShadersAssets.Any(sa => sa.AssetName == a.AssetName))); // Or be a shader not present in core, with a fragment file.

			_audioControl = new(this, BinaryFileType.Audio, AssetType.Audio, hasAudioAssets, UserHandler.Instance.Cache.MakeBinaryAudioName, true);
			_coreControl = new(this, BinaryFileType.Core, AssetType.Shader, hasCoreAssets, "core", false);
			_ddControl = new(this, BinaryFileType.Dd, AssetType.Texture, hasDdAssets, UserHandler.Instance.Cache.MakeBinaryDdName, true);

			_controls.Add(_audioControl);
			_controls.Add(_coreControl);
			_controls.Add(_ddControl);

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
					}

					BinaryFileHandler.MakeBinary(assetTabControls.SelectMany(atc => atc.GetAssets()).ToList(), control.OutputPath, control.Progress);

					App.Instance.Dispatcher.Invoke(() => control.Progress.Report("Completed successfully.", 1));
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Making binary did not complete successfully", $"An error occurred while making '{control.BinaryFileType.ToString().ToLower()}' binary.", ex);
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

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			UserHandler.Instance.Cache.MakeBinaryAudioName = _audioControl.BinaryName;
			UserHandler.Instance.Cache.MakeBinaryDdName = _ddControl.BinaryName;
			UserHandler.Instance.Cache.MakeBinaryName = TextBoxModName.Text;
		}
	}
}
