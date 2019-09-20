using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.Web;
using DevilDaggersAssetEditor.GUI.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetCore.Assets;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using DevilDaggersAssetEditor.Code.ExpanderControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		private readonly ResourceFileHandler audioFileHandler = new ResourceFileHandler(BinaryFileType.Audio);
		private readonly ResourceFileHandler ddFileHandler = new ResourceFileHandler(BinaryFileType.DD);
		private readonly ResourceFileHandler coreFileHandler = new ResourceFileHandler(BinaryFileType.Core);

		private readonly List<AbstractBinaryFileHandler> fileHandlers;

		public MenuBarUserControl()
		{
			InitializeComponent();

			fileHandlers = new List<AbstractBinaryFileHandler>
			{
				audioFileHandler,
				ddFileHandler,
				coreFileHandler
			};

			foreach (AbstractBinaryFileHandler fileHandler in fileHandlers)
			{
				string fileName = fileHandler.BinaryFileType.ToString().ToLower();
				MenuItem bfnItem = new MenuItem { Header = fileName, IsEnabled = fileHandler.BinaryFileType != BinaryFileType.Particle };

				MenuItem extractItem = new MenuItem { Header = $"Extract '{fileName}'" };
				MenuItem compressItem = new MenuItem { Header = $"Compress '{fileName}'", IsEnabled = fileHandler.BinaryFileType == BinaryFileType.Audio };
				extractItem.Click += (sender, e) => Extract_Click(fileHandler);
				compressItem.Click += (sender, e) => Compress_Click(fileHandler);
				bfnItem.Items.Add(extractItem);
				bfnItem.Items.Add(compressItem);

				bfnItem.Items.Add(new Separator());

				MenuItem openItem = new MenuItem { Header = "Open assets", IsEnabled = fileHandler.BinaryFileType == BinaryFileType.Audio };
				MenuItem saveItem = new MenuItem { Header = "Save assets", IsEnabled = fileHandler.BinaryFileType == BinaryFileType.Audio };

				switch (fileHandler.BinaryFileType)
				{
					case BinaryFileType.Audio:
						openItem.Click += (sender, e) => SetAudioAssets(Open());
						saveItem.Click += (sender, e) => Save(GetAudioAssets(), fileHandler);

						AddOpenSaveItems();

						MenuItem audioAudioImport = new MenuItem { Header = $"Import Audio paths from folder" };
						audioAudioImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioExpanderControl.Handler.ImportFolder();
						bfnItem.Items.Add(audioAudioImport);

						bfnItem.Items.Add(new Separator());

						MenuItem loudnessImport = new MenuItem { Header = $"Import Loudness from file" };
						MenuItem loudnessExport = new MenuItem { Header = $"Export Loudness to file" };
						loudnessImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioExpanderControl.Handler.ImportLoudness();
						loudnessExport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioExpanderControl.Handler.ExportLoudness();
						bfnItem.Items.Add(loudnessImport);
						bfnItem.Items.Add(loudnessExport);
						break;
					case BinaryFileType.DD:
						openItem.Click += (sender, e) => SetDDAssets(Open());
						saveItem.Click += (sender, e) => Save(GetDDAssets(), fileHandler);

						AddOpenSaveItems();

						MenuItem ddModelBindingImport = new MenuItem { Header = $"Import Model Binding paths from folder", IsEnabled = false };
						MenuItem ddModelImport = new MenuItem { Header = $"Import Model paths from folder", IsEnabled = false };
						MenuItem ddShaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };
						MenuItem ddTextureImport = new MenuItem { Header = $"Import Texture paths from folder", IsEnabled = false };
						ddModelBindingImport.Click += (sender, e) => App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.ImportFolder();
						ddModelImport.Click += (sender, e) => App.Instance.MainWindow.DDModelsExpanderControl.Handler.ImportFolder();
						ddShaderImport.Click += (sender, e) => App.Instance.MainWindow.DDShadersExpanderControl.Handler.ImportFolder();
						ddTextureImport.Click += (sender, e) => App.Instance.MainWindow.DDTexturesExpanderControl.Handler.ImportFolder();
						bfnItem.Items.Add(ddModelBindingImport);
						bfnItem.Items.Add(ddModelImport);
						bfnItem.Items.Add(ddShaderImport);
						bfnItem.Items.Add(ddTextureImport);
						break;
					case BinaryFileType.Core:
						openItem.Click += (sender, e) => SetCoreAssets(Open());
						saveItem.Click += (sender, e) => Save(GetCoreAssets(), fileHandler);

						AddOpenSaveItems();

						MenuItem coreShaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };
						coreShaderImport.Click += (sender, e) => App.Instance.MainWindow.CoreShadersExpanderControl.Handler.ImportFolder(); ;
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

#if DEBUG
			MenuItem testException = new MenuItem { Header = "Test Exception", Background = new SolidColorBrush(Color.FromRgb(0, 255, 64)) };
			testException.Click += TestException_Click;

			MenuItem debug = new MenuItem { Header = "Debug", Background = new SolidColorBrush(Color.FromRgb(0, 255, 64)) };
			debug.Items.Add(testException);

			MenuPanel.Items.Add(debug);
#endif
		}

		private List<GenericUserAsset> Open()
		{
			OpenFileDialog dialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder };
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return null;

			return JsonUtils.TryDeserializeFromFile<List<GenericUserAsset>>(dialog.FileName, true);
		}

		private void Save(List<AbstractAsset> assets, AbstractBinaryFileHandler fileHandler)
		{
			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Utils.DDFolder, AddExtension = true, DefaultExt = fileHandler.BinaryFileType.ToString().ToLower() };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			List<GenericUserAsset> userAssets = new List<GenericUserAsset>();
			foreach (AbstractAsset asset in assets)
				userAssets.Add(asset is AudioAsset audioAsset ? new AudioUserAsset(audioAsset.AssetName, audioAsset.EditorPath, audioAsset.Loudness) : new GenericUserAsset(asset.AssetName, asset.EditorPath));

			JsonUtils.SerializeToFile(dialog.FileName, userAssets, true, Formatting.None);
		}

		private void Extract_Click(AbstractBinaryFileHandler fileHandler)
		{
			OpenFileDialog openDialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			using (CommonOpenFileDialog saveDialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Utils.DDFolder })
			{
				CommonFileDialogResult saveResult = saveDialog.ShowDialog();
				if (saveResult == CommonFileDialogResult.Ok)
					fileHandler.Extract(openDialog.FileName, saveDialog.FileName);
			}
		}

		private void Compress_Click(AbstractBinaryFileHandler fileHandler)
		{
			switch (fileHandler.BinaryFileType)
			{
				case BinaryFileType.Audio:
					if (!App.Instance.MainWindow.AudioAudioExpanderControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetAudioAssets());
					break;
				case BinaryFileType.DD:
					if (!App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.IsComplete()
					 || !App.Instance.MainWindow.DDModelsExpanderControl.Handler.IsComplete()
					 || !App.Instance.MainWindow.DDShadersExpanderControl.Handler.IsComplete()
					 || !App.Instance.MainWindow.DDTexturesExpanderControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetDDAssets());
					break;
				case BinaryFileType.Core:
					if (!App.Instance.MainWindow.CoreShadersExpanderControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetCoreAssets());
					break;
				default:
					throw new Exception($"Method '{nameof(Compress_Click)}' not implemented for {nameof(BinaryFileType)} '{fileHandler.BinaryFileType}'.");
			}

			void Compress(List<AbstractAsset> assets)
			{
				SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Path.Combine(Utils.DDFolder, fileHandler.BinaryFileType.GetSubfolderName()) };
				bool? result = dialog.ShowDialog();
				if (!result.HasValue || !result.Value)
					return;

				fileHandler.Compress(assets, dialog.FileName);
			}
		}

		private List<AbstractAsset> GetAudioAssets()
		{
			return App.Instance.MainWindow.AudioAudioExpanderControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		private List<AbstractAsset> GetDDAssets()
		{
			return App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.Assets.Cast<AbstractAsset>()
				.Concat(App.Instance.MainWindow.DDModelsExpanderControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDShadersExpanderControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDTexturesExpanderControl.Handler.Assets.Cast<AbstractAsset>())
				.ToList();
		}

		private List<AbstractAsset> GetCoreAssets()
		{
			return App.Instance.MainWindow.CoreShadersExpanderControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		private void SetAudioAssets(List<GenericUserAsset> assets)
		{
			if (assets == null)
				return;

			SetAssets(assets.Cast<AudioUserAsset>().ToList(), App.Instance.MainWindow.AudioAudioExpanderControl.Handler);
		}

		private void SetDDAssets(List<GenericUserAsset> assets)
		{
			if (assets == null)
				return;

			SetAssets(assets, App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler);
			SetAssets(assets, App.Instance.MainWindow.DDModelsExpanderControl.Handler);
			SetAssets(assets, App.Instance.MainWindow.DDShadersExpanderControl.Handler);
			SetAssets(assets, App.Instance.MainWindow.DDTexturesExpanderControl.Handler);
		}

		private void SetCoreAssets(List<GenericUserAsset> assets)
		{
			if (assets == null)
				return;

			SetAssets(assets, App.Instance.MainWindow.CoreShadersExpanderControl.Handler);
		}

		private void SetAssets<TUserAsset, TAsset, TAssetControl>(List<TUserAsset> userAssets, AbstractExpanderControlHandler<TAsset, TAssetControl> assetHandler) where TUserAsset : GenericUserAsset where TAsset : AbstractAsset where TAssetControl : UserControl
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

					assetHandler.UpdateGUI(asset);
				}
			}
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private void SourceCode_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.SourceCode);
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new CheckingForUpdatesWindow();
			window.ShowDialog();

			if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue)
			{
				if (!NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
					updateRecommendedWindow.ShowDialog();
				}
				else
				{
					App.Instance.ShowMessage("Up to date", $"{ApplicationUtils.ApplicationDisplayNameWithVersion} is up to date.");
				}
			}
		}

		private void ShowLog_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists("DDAE.log"))
				Process.Start("DDAE.log");
			else
				App.Instance.ShowMessage("No log file", "Log file does not exist.");
		}

		private void TestException_Click(object sender, RoutedEventArgs e)
		{
			throw new Exception("Test Exception");
		}
	}
}