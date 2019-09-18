using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Assets.UserAssets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public BinaryFileName activeBinaryFileName;

		public MainWindow()
		{
			InitializeComponent();

			foreach (BinaryFileName binaryFileName in (BinaryFileName[])Enum.GetValues(typeof(BinaryFileName)))
			{
				string bfn = binaryFileName.ToString().ToLower();
				MenuItem bfnItem = new MenuItem { Header = bfn, IsEnabled = binaryFileName != BinaryFileName.Particle };

				MenuItem extractItem = new MenuItem { Header = $"Extract '{bfn}'" };
				MenuItem compressItem = new MenuItem { Header = $"Compress '{bfn}'" };
				extractItem.Click += (sender, e) => Extract_Click(binaryFileName);
				compressItem.Click += (sender, e) => Compress_Click(binaryFileName);
				bfnItem.Items.Add(extractItem);
				bfnItem.Items.Add(compressItem);

				bfnItem.Items.Add(new Separator());

				MenuItem openItem = new MenuItem { Header = "Open assets" };
				MenuItem saveItem = new MenuItem { Header = "Save assets" };

				switch (binaryFileName)
				{
					case BinaryFileName.Audio:
						openItem.Click += (sender, e) => SetAudioAssets(Open());
						saveItem.Click += (sender, e) => Save(GetAudioAssets(), binaryFileName);

						AddOpenSaveItems();

						MenuItem audioAudioImport = new MenuItem { Header = $"Import Audio paths from folder" };
						audioAudioImport.Click += (sender, e) => AudioAudioTabControl.Handler.ImportFolder();
						bfnItem.Items.Add(audioAudioImport);

						bfnItem.Items.Add(new Separator());

						MenuItem loudnessImport = new MenuItem { Header = $"Import loudness from file" };
						MenuItem loudnessExport = new MenuItem { Header = $"Export loudness to file" };
						loudnessImport.Click += (sender, e) => AudioAudioTabControl.Handler.ImportLoudness();
						loudnessExport.Click += (sender, e) => AudioAudioTabControl.Handler.ExportLoudness();
						bfnItem.Items.Add(loudnessImport);
						bfnItem.Items.Add(loudnessExport);
						break;
					case BinaryFileName.DD:
						openItem.Click += (sender, e) => SetDDAssets(Open());
						saveItem.Click += (sender, e) => Save(GetDDAssets(), binaryFileName);

						AddOpenSaveItems();

						MenuItem ddModelBindingImport = new MenuItem { Header = $"Import Model Binding paths from folder" };
						MenuItem ddModelImport = new MenuItem { Header = $"Import Model paths from folder" };
						MenuItem ddShaderImport = new MenuItem { Header = $"Import Shader paths from folder" };
						MenuItem ddTextureImport = new MenuItem { Header = $"Import Texture paths from folder" };
						ddModelBindingImport.Click += (sender, e) => DDModelBindingsTabControl.Handler.ImportFolder();
						ddModelImport.Click += (sender, e) => DDModelsTabControl.Handler.ImportFolder();
						ddShaderImport.Click += (sender, e) => DDShadersTabControl.Handler.ImportFolder();
						ddTextureImport.Click += (sender, e) => DDTexturesTabControl.Handler.ImportFolder();
						bfnItem.Items.Add(ddModelBindingImport);
						bfnItem.Items.Add(ddModelImport);
						bfnItem.Items.Add(ddShaderImport);
						bfnItem.Items.Add(ddTextureImport);
						break;
					case BinaryFileName.Core:
						openItem.Click += (sender, e) => SetCoreAssets(Open());
						saveItem.Click += (sender, e) => Save(GetCoreAssets(), binaryFileName);

						AddOpenSaveItems();

						MenuItem coreShaderImport = new MenuItem { Header = $"Import Shader paths from folder" };
						coreShaderImport.Click += (sender, e) => CoreShadersTabControl.Handler.ImportFolder(); ;
						bfnItem.Items.Add(coreShaderImport);
						break;
				}

				FileMenuItem.Items.Add(bfnItem);

				void AddOpenSaveItems()
				{
					bfnItem.Items.Add(openItem);
					bfnItem.Items.Add(saveItem);
					bfnItem.Items.Add(new Separator());
				}
			}
		}

		private List<GenericUserAsset> Open()
		{
			OpenFileDialog dialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder };
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return null;

			return JsonUtils.DeserializeFromFile<List<GenericUserAsset>>(dialog.FileName, true);
		}

		private void Save(List<AbstractAsset> assets, BinaryFileName binaryFileName)
		{
			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Utils.DDFolder, AddExtension = true, DefaultExt = binaryFileName.ToString().ToLower() };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			List<GenericUserAsset> userAssets = new List<GenericUserAsset>();
			foreach (AbstractAsset asset in assets)
				userAssets.Add(asset is AudioAsset audioAsset ? new AudioUserAsset(audioAsset.AssetName, audioAsset.EditorPath, audioAsset.Loudness) : new GenericUserAsset(asset.AssetName, asset.EditorPath));

			JsonUtils.SerializeToFile(dialog.FileName, userAssets, true, Formatting.None);
		}

		private void Extract_Click(BinaryFileName binaryFileName)
		{
			OpenFileDialog openDialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			using (CommonOpenFileDialog saveDialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Utils.DDFolder })
			{
				CommonFileDialogResult saveResult = saveDialog.ShowDialog();
				if (saveResult == CommonFileDialogResult.Ok)
					Extractor.Extract(openDialog.FileName, saveDialog.FileName, binaryFileName);
			}
		}

		private void Compress_Click(BinaryFileName binaryFileName)
		{
			switch (binaryFileName)
			{
				case BinaryFileName.Audio:
					if (!AudioAudioTabControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetAudioAssets());
					break;
				case BinaryFileName.DD:
					if (!DDModelBindingsTabControl.Handler.IsComplete()
					 || !DDModelsTabControl.Handler.IsComplete()
					 || !DDShadersTabControl.Handler.IsComplete()
					 || !DDTexturesTabControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetDDAssets());
					break;
				case BinaryFileName.Core:
					if (!CoreShadersTabControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetCoreAssets());
					break;
				default:
					throw new Exception($"Method '{nameof(Compress_Click)}' not implemented for {nameof(BinaryFileName)} '{binaryFileName}'.");
			}

			void Compress(List<AbstractAsset> assets)
			{
				SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Path.Combine(Utils.DDFolder, binaryFileName.GetSubfolderName()) };
				bool? result = dialog.ShowDialog();
				if (!result.HasValue || !result.Value)
					return;

				Compressor.Compress(assets, dialog.FileName, binaryFileName);
			}
		}

		private List<AbstractAsset> GetAudioAssets()
		{
			return AudioAudioTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		private List<AbstractAsset> GetDDAssets()
		{
			return DDModelBindingsTabControl.Handler.Assets.Cast<AbstractAsset>()
				.Concat(DDModelsTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(DDShadersTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(DDTexturesTabControl.Handler.Assets.Cast<AbstractAsset>())
				.ToList();
		}

		private List<AbstractAsset> GetCoreAssets()
		{
			return CoreShadersTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		private void SetAudioAssets(List<GenericUserAsset> assets)
		{
			SetAssets(assets.Cast<AudioUserAsset>().ToList(), AudioAudioTabControl.Handler);
		}

		private void SetDDAssets(List<GenericUserAsset> assets)
		{
			SetAssets(assets, DDModelBindingsTabControl.Handler);
			SetAssets(assets, DDModelsTabControl.Handler);
			SetAssets(assets, DDShadersTabControl.Handler);
			SetAssets(assets, DDTexturesTabControl.Handler);
		}

		private void SetCoreAssets(List<GenericUserAsset> assets)
		{
			SetAssets(assets, CoreShadersTabControl.Handler);
		}

		private void SetAssets<TUserAsset, TAsset, TAssetControl>(List<TUserAsset> userAssets, AbstractTabControlHandler<TAsset, TAssetControl> assetHandler) where TUserAsset : GenericUserAsset where TAsset : AbstractAsset
		{
			for (int i = 0; i < assetHandler.Assets.Count; i++)
			{
				TAsset asset = assetHandler.Assets[i];
				TUserAsset userAsset = userAssets.Where(a => a.AssetName == asset.AssetName).FirstOrDefault();
				if (userAsset != null)
				{
					asset.EditorPath = userAsset.EditorPath;

					// I am the worst programmer on the planet.
					if (asset is AudioAsset audioAsset && userAsset is AudioUserAsset audioUserAsset)
						audioAsset.Loudness = audioUserAsset.Loudness;

					assetHandler.UpdatePathLabel(asset);
				}
			}
		}
	}
}