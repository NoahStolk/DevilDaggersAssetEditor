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
using DevilDaggersAssetCore.Assets.UserAssets;
using DevilDaggersAssetEditor.Code.TabControlHandlers;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.GUI.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		public MenuBarUserControl()
		{
			InitializeComponent();

			foreach (BinaryFileName binaryFileName in (BinaryFileName[])Enum.GetValues(typeof(BinaryFileName)))
			{
				string bfn = binaryFileName.ToString().ToLower();
				MenuItem bfnItem = new MenuItem { Header = bfn, IsEnabled = binaryFileName != BinaryFileName.Particle };

				MenuItem extractItem = new MenuItem { Header = $"Extract '{bfn}'" };
				MenuItem compressItem = new MenuItem { Header = $"Compress '{bfn}'", IsEnabled = binaryFileName == BinaryFileName.Audio };
				extractItem.Click += (sender, e) => Extract_Click(binaryFileName);
				compressItem.Click += (sender, e) => Compress_Click(binaryFileName);
				bfnItem.Items.Add(extractItem);
				bfnItem.Items.Add(compressItem);

				bfnItem.Items.Add(new Separator());

				MenuItem openItem = new MenuItem { Header = "Open assets", IsEnabled = binaryFileName == BinaryFileName.Audio };
				MenuItem saveItem = new MenuItem { Header = "Save assets", IsEnabled = binaryFileName == BinaryFileName.Audio };

				switch (binaryFileName)
				{
					case BinaryFileName.Audio:
						openItem.Click += (sender, e) => SetAudioAssets(Open());
						saveItem.Click += (sender, e) => Save(GetAudioAssets(), binaryFileName);

						AddOpenSaveItems();

						MenuItem audioAudioImport = new MenuItem { Header = $"Import Audio paths from folder" };
						audioAudioImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioTabControl.Handler.ImportFolder();
						bfnItem.Items.Add(audioAudioImport);

						bfnItem.Items.Add(new Separator());

						MenuItem loudnessImport = new MenuItem { Header = $"Import Loudness from file" };
						MenuItem loudnessExport = new MenuItem { Header = $"Export Loudness to file" };
						loudnessImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioTabControl.Handler.ImportLoudness();
						loudnessExport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioTabControl.Handler.ExportLoudness();
						bfnItem.Items.Add(loudnessImport);
						bfnItem.Items.Add(loudnessExport);
						break;
					case BinaryFileName.DD:
						openItem.Click += (sender, e) => SetDDAssets(Open());
						saveItem.Click += (sender, e) => Save(GetDDAssets(), binaryFileName);

						AddOpenSaveItems();

						MenuItem ddModelBindingImport = new MenuItem { Header = $"Import Model Binding paths from folder", IsEnabled = false };
						MenuItem ddModelImport = new MenuItem { Header = $"Import Model paths from folder", IsEnabled = false };
						MenuItem ddShaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };
						MenuItem ddTextureImport = new MenuItem { Header = $"Import Texture paths from folder", IsEnabled = false };
						ddModelBindingImport.Click += (sender, e) => App.Instance.MainWindow.DDModelBindingsTabControl.Handler.ImportFolder();
						ddModelImport.Click += (sender, e) => App.Instance.MainWindow.DDModelsTabControl.Handler.ImportFolder();
						ddShaderImport.Click += (sender, e) => App.Instance.MainWindow.DDShadersTabControl.Handler.ImportFolder();
						ddTextureImport.Click += (sender, e) => App.Instance.MainWindow.DDTexturesTabControl.Handler.ImportFolder();
						bfnItem.Items.Add(ddModelBindingImport);
						bfnItem.Items.Add(ddModelImport);
						bfnItem.Items.Add(ddShaderImport);
						bfnItem.Items.Add(ddTextureImport);
						break;
					case BinaryFileName.Core:
						openItem.Click += (sender, e) => SetCoreAssets(Open());
						saveItem.Click += (sender, e) => Save(GetCoreAssets(), binaryFileName);

						AddOpenSaveItems();

						MenuItem coreShaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };
						coreShaderImport.Click += (sender, e) => App.Instance.MainWindow.CoreShadersTabControl.Handler.ImportFolder(); ;
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
					if (!App.Instance.MainWindow.AudioAudioTabControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetAudioAssets());
					break;
				case BinaryFileName.DD:
					if (!App.Instance.MainWindow.DDModelBindingsTabControl.Handler.IsComplete()
					 || !App.Instance.MainWindow.DDModelsTabControl.Handler.IsComplete()
					 || !App.Instance.MainWindow.DDShadersTabControl.Handler.IsComplete()
					 || !App.Instance.MainWindow.DDTexturesTabControl.Handler.IsComplete())
					{
						MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (promptResult == MessageBoxResult.No)
							return;
					}

					Compress(GetDDAssets());
					break;
				case BinaryFileName.Core:
					if (!App.Instance.MainWindow.CoreShadersTabControl.Handler.IsComplete())
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
			return App.Instance.MainWindow.AudioAudioTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		private List<AbstractAsset> GetDDAssets()
		{
			return App.Instance.MainWindow.DDModelBindingsTabControl.Handler.Assets.Cast<AbstractAsset>()
				.Concat(App.Instance.MainWindow.DDModelsTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDShadersTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDTexturesTabControl.Handler.Assets.Cast<AbstractAsset>())
				.ToList();
		}

		private List<AbstractAsset> GetCoreAssets()
		{
			return App.Instance.MainWindow.CoreShadersTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		private void SetAudioAssets(List<GenericUserAsset> assets)
		{
			SetAssets(assets.Cast<AudioUserAsset>().ToList(), App.Instance.MainWindow.AudioAudioTabControl.Handler);
		}

		private void SetDDAssets(List<GenericUserAsset> assets)
		{
			SetAssets(assets, App.Instance.MainWindow.DDModelBindingsTabControl.Handler);
			SetAssets(assets, App.Instance.MainWindow.DDModelsTabControl.Handler);
			SetAssets(assets, App.Instance.MainWindow.DDShadersTabControl.Handler);
			SetAssets(assets, App.Instance.MainWindow.DDTexturesTabControl.Handler);
		}

		private void SetCoreAssets(List<GenericUserAsset> assets)
		{
			SetAssets(assets, App.Instance.MainWindow.CoreShadersTabControl.Handler);
		}

		private void SetAssets<TUserAsset, TAsset, TAssetControl>(List<TUserAsset> userAssets, AbstractTabControlHandler<TAsset, TAssetControl> assetHandler) where TUserAsset : GenericUserAsset where TAsset : AbstractAsset where TAssetControl : UserControl
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