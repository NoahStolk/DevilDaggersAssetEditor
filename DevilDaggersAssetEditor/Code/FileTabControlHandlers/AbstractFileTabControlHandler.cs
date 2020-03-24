using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;
using DevilDaggersAssetEditor.Code.User;
using DevilDaggersAssetEditor.Gui.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
{
	public abstract class AbstractFileTabControlHandler
	{
		public abstract AbstractBinaryFileHandler FileHandler { get; }

		public virtual MenuItem CreateFileTypeMenuItem()
		{
			BinaryFileType binaryFileType = FileHandler.BinaryFileType;
			string fileName = binaryFileType.ToString().ToLower();

			MenuItem extractItem = new MenuItem { Header = $"Extract '{fileName}'" };
			MenuItem compressItem = new MenuItem { Header = $"Compress '{fileName}'" };
			MenuItem openModFileItem = new MenuItem { Header = $"Open .{fileName} mod file" };
			MenuItem saveModFileItem = new MenuItem { Header = $"Save .{fileName} mod file" };

			extractItem.Click += (sender, e) => Extract_Click();
			compressItem.Click += (sender, e) => Compress_Click();
			openModFileItem.Click += (sender, e) =>
			{
				ModFile modFile = OpenModFile();
				if (modFile == null)
					return;
				UpdateAssetTabControls(modFile.Assets);
			};
			saveModFileItem.Click += (sender, e) =>
			{
				List<AbstractAsset> assets = GetAssets();
				List<AbstractUserAsset> userAssets = CreateUserAssets(assets);
				SaveModFile(userAssets);
			};

			MenuItem fileTypeMenuItem = new MenuItem { Header = fileName };

			fileTypeMenuItem.Items.Add(extractItem);
			fileTypeMenuItem.Items.Add(compressItem);
			fileTypeMenuItem.Items.Add(new Separator());
			fileTypeMenuItem.Items.Add(openModFileItem);
			fileTypeMenuItem.Items.Add(saveModFileItem);
			fileTypeMenuItem.Items.Add(new Separator());

			return fileTypeMenuItem;
		}

		protected abstract bool IsComplete();

		public abstract List<AbstractAsset> GetAssets();

		private async void Extract_Click()
		{
			OpenFileDialog openDialog = new OpenFileDialog { InitialDirectory = Path.Combine(UserHandler.Instance.settings.DevilDaggersRootFolder, FileHandler.BinaryFileType.GetSubfolderName()) };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = UserHandler.Instance.settings.AssetsRootFolder })
			{
				if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
				{
					ProgressWindow progressWindow = new ProgressWindow($"Extracting '{FileHandler.BinaryFileType.ToString().ToLower()}'...");
					progressWindow.Show();
					await Task.Run(() =>
					{
						FileHandler.Extract(
							openDialog.FileName,
							folderDialog.FileName,
							new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressBar.Value = value)),
							new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressDescription.Text = value)));
						App.Instance.Dispatcher.Invoke(() => progressWindow.Finish());
					});
				}
			}
		}

		private async void Compress_Click()
		{
			if (!IsComplete())
			{
				MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up (or sometimes randomly at runtime). Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (promptResult == MessageBoxResult.No)
					return;
			}

			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Path.Combine(UserHandler.Instance.settings.DevilDaggersRootFolder, FileHandler.BinaryFileType.GetSubfolderName()) };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			ProgressWindow progressWindow = new ProgressWindow($"Compressing '{FileHandler.BinaryFileType.ToString().ToLower()}'...");
			progressWindow.Show();
			await Task.Run(() =>
			{
				try
				{
					FileHandler.Compress(
						allAssets: GetAssets(),
						outputPath: dialog.FileName,
						progress: new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressBar.Value = value)),
						progressDescription: new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressDescription.Text = value)));

					App.Instance.Dispatcher.Invoke(() => progressWindow.Finish());
				}
				catch (Exception ex)
				{
					App.Instance.Dispatcher.Invoke(() =>
					{
						App.Instance.ShowError("Compression failed", $"Compressing assets failed during the execution of \"{progressWindow.ProgressDescription.Text}\".", ex);
						progressWindow.Error();
					});
				}
			});
		}

		private void SaveModFile(List<AbstractUserAsset> assets)
		{
			string modFileExtension = FileHandler.BinaryFileType.ToString().ToLower();
			string modFileFilter = $"{FileHandler.BinaryFileType} mod files (*.{modFileExtension})|*.{modFileExtension}";
			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = UserHandler.Instance.settings.ModsRootFolder, Filter = modFileFilter };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			bool samePaths = true;
			List<AbstractUserAsset> list = assets.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				string path1 = list[i].EditorPath;
				string path2 = list[(i + 1) % list.Count].EditorPath;
				if (!path1.IsPathValid() || !path2.IsPathValid() || Path.GetDirectoryName(path1) != Path.GetDirectoryName(path2))
				{
					samePaths = false;
					break;
				}
			}

			bool relativePaths = false;
			if (samePaths)
			{
				MessageBoxResult relativePathsResult = MessageBox.Show("Specify whether you want this mod file to use relative paths (easier to share between computers or using zipped files containing assets).", "Use relative paths?", MessageBoxButton.YesNo, MessageBoxImage.Question);
				relativePaths = relativePathsResult == MessageBoxResult.Yes;

				if (relativePaths)
					foreach (AbstractUserAsset asset in assets)
						asset.EditorPath = Path.GetFileName(asset.EditorPath);
			}
			ModFile modFile = new ModFile(App.LocalVersion, relativePaths, assets);

			JsonUtils.SerializeToFile(dialog.FileName, modFile, true);
		}

		private List<AbstractUserAsset> CreateUserAssets(List<AbstractAsset> assets)
		{
			List<AbstractUserAsset> userAssets = new List<AbstractUserAsset>();
			foreach (AbstractAsset asset in assets)
				userAssets.Add(asset.ToUserAsset());
			return userAssets;
		}

		private ModFile OpenModFile()
		{
			string modFileExtension = FileHandler.BinaryFileType.ToString().ToLower();
			string modFileFilter = $"{FileHandler.BinaryFileType} mod files (*.{modFileExtension})|*.{modFileExtension}";
			OpenFileDialog dialog = new OpenFileDialog { InitialDirectory = UserHandler.Instance.settings.ModsRootFolder, Filter = modFileFilter };
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return null;

			ModFile modFile = JsonUtils.TryDeserializeFromFile<ModFile>(dialog.FileName, true);
			if (modFile == null)
				App.Instance.ShowMessage("Mod not loaded", "Could not parse mod file.");

			if (modFile.HasRelativePaths)
			{
				App.Instance.ShowMessage("Specify base path", "This mod file uses relative paths. Please specify a base path.");
				using (CommonOpenFileDialog basePathDialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = UserHandler.Instance.settings.AssetsRootFolder })
				{
					CommonFileDialogResult result = basePathDialog.ShowDialog();
					if (result == CommonFileDialogResult.Ok)
						foreach (AbstractUserAsset asset in modFile.Assets)
							asset.EditorPath = Path.Combine(basePathDialog.FileName, asset.EditorPath);
				}
			}
			return modFile;
		}

		protected abstract void UpdateAssetTabControls(List<AbstractUserAsset> assets);

		protected void UpdateAssetTabControl<TUserAsset, TAsset, TAssetControl>(List<TUserAsset> userAssets, AbstractAssetTabControlHandler<TAsset, TAssetControl> assetTabControlHandler)
			where TUserAsset : AbstractUserAsset
			where TAsset : AbstractAsset
			where TAssetControl : UserControl
		{
			for (int i = 0; i < assetTabControlHandler.Assets.Count; i++)
			{
				TAsset asset = assetTabControlHandler.Assets[i];
				TUserAsset userAsset = userAssets.Where(a => a.AssetName == asset.AssetName && (asset.ChunkTypeName == null /*particles don't have a chunk type*/ || a.ChunkTypeName == asset.ChunkTypeName)).FirstOrDefault();
				if (userAsset != null)
				{
					asset.ImportValuesFromUserAsset(userAsset);

					assetTabControlHandler.UpdateGui(asset);
				}
			}
		}
	}
}