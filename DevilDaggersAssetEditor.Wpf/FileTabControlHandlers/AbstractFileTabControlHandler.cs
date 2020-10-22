using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.FileTabControlHandlers
{
	public abstract class AbstractFileTabControlHandler
	{
		public abstract AbstractBinaryFileHandler FileHandler { get; }

		public virtual MenuItem CreateFileTypeMenuItem()
		{
			BinaryFileType binaryFileType = FileHandler.BinaryFileType;
			string fileName = binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture);

			MenuItem extractBinaryItem = new MenuItem { Header = $"Extract '{fileName}' binary" };
			MenuItem makeBinaryItem = new MenuItem { Header = $"Make '{fileName}' binary" };
			MenuItem openModFileItem = new MenuItem { Header = $"Open .{fileName} mod file" };
			MenuItem saveModFileItem = new MenuItem { Header = $"Save .{fileName} mod file" };

			extractBinaryItem.Click += async (sender, e) => await ExtractBinary_Click();
			makeBinaryItem.Click += async (sender, e) => await MakeBinary_Click();
			openModFileItem.Click += (sender, e) =>
			{
				List<UserAsset> assets = OpenModFile();
				if (assets.Count == 0)
					return;
				UpdateAssetTabControls(assets);
			};
			saveModFileItem.Click += (sender, e) =>
			{
				List<AbstractAsset> assets = GetAssets();
				List<UserAsset> userAssets = CreateUserAssets(assets);
				SaveModFile(userAssets);
			};

			MenuItem fileTypeMenuItem = new MenuItem { Header = fileName };

			fileTypeMenuItem.Items.Add(extractBinaryItem);
			fileTypeMenuItem.Items.Add(makeBinaryItem);
			fileTypeMenuItem.Items.Add(new Separator());
			fileTypeMenuItem.Items.Add(openModFileItem);
			fileTypeMenuItem.Items.Add(saveModFileItem);
			fileTypeMenuItem.Items.Add(new Separator());

			return fileTypeMenuItem;
		}

		protected abstract bool IsComplete();

		public abstract List<AbstractAsset> GetAssets();

		private async Task ExtractBinary_Click()
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			string initDir = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, FileHandler.BinaryFileType.GetSubfolderName());
			if (UserHandler.Instance.Settings.EnableDevilDaggersRootFolder && Directory.Exists(initDir))
				openDialog.InitialDirectory = initDir;

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
			if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.ModsRootFolder))
				folderDialog.SelectedPath = $"{UserHandler.Instance.Settings.ModsRootFolder}\\";

			if (folderDialog.ShowDialog() == true)
			{
				ProgressWindow progressWindow = new ProgressWindow($"Extracting '{FileHandler.BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}'...");
				progressWindow.Show();
				await Task.Run(() =>
				{
					try
					{
						FileHandler.ExtractBinary(
							openDialog.FileName,
							folderDialog.SelectedPath,
							FileHandler.BinaryFileType,
							new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressBar.Value = value)),
							new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => progressWindow.ProgressDescription.Text = value)));
						App.Instance.Dispatcher.Invoke(() => progressWindow.Finish());
					}
					catch (Exception ex)
					{
						App.Instance.Dispatcher.Invoke(() =>
						{
							App.Instance.ShowError("Extracting binary did not complete successfully", $"An error occurred during the execution of \"{progressWindow.ProgressDescription.Text}\".", ex);
							progressWindow.Error();
						});
					}
				});
			}
		}

		private async Task MakeBinary_Click()
		{
			if (!IsComplete())
			{
				ConfirmWindow confirmWindow = new ConfirmWindow("Incomplete asset list", "Not all file paths have been specified. This will cause Devil Daggers to crash when it attempts to load the unspecified asset. Are you sure you wish to continue?", false);
				confirmWindow.ShowDialog();
				if (!confirmWindow.IsConfirmed)
					return;
			}

			SaveFileDialog dialog = new SaveFileDialog();
			string initDir = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, FileHandler.BinaryFileType.GetSubfolderName());
			if (UserHandler.Instance.Settings.EnableDevilDaggersRootFolder && Directory.Exists(initDir))
				dialog.InitialDirectory = initDir;

			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			ProgressWindow progressWindow = new ProgressWindow($"Turning files into '{FileHandler.BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' binary...");
			progressWindow.Show();
			await Task.Run(() =>
			{
				try
				{
					FileHandler.MakeBinary(
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
						App.Instance.ShowError("Making binary did not complete successfully", $"An error occurred during the execution of \"{progressWindow.ProgressDescription.Text}\".", ex);
						progressWindow.Error();
					});
				}
			});
		}

		private void SaveModFile(List<UserAsset> assets)
		{
			string modFileExtension = FileHandler.BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture);
			string modFileFilter = $"{FileHandler.BinaryFileType} mod files (*.{modFileExtension})|*.{modFileExtension}";
			SaveFileDialog dialog = new SaveFileDialog { Filter = modFileFilter };
			if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.ModsRootFolder))
				dialog.InitialDirectory = UserHandler.Instance.Settings.ModsRootFolder;

			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			JsonFileUtils.SerializeToFile(dialog.FileName, assets, true);
		}

		private static List<UserAsset> CreateUserAssets(List<AbstractAsset> assets)
		{
			List<UserAsset> userAssets = new List<UserAsset>();
			foreach (AbstractAsset asset in assets)
				userAssets.Add(asset.ToUserAsset());
			return userAssets;
		}

		private static List<UserAsset> OpenModFile()
		{
			OpenFileDialog dialog = new OpenFileDialog { Filter = "Devil Daggers Asset Editor mod files (*.ddae)|*.ddae" };
			if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.ModsRootFolder))
				dialog.InitialDirectory = UserHandler.Instance.Settings.ModsRootFolder;

			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return new List<UserAsset>();

			return ModFileUtils.GetAssetsFromModFilePath(dialog.FileName);
		}

		public abstract void UpdateAssetTabControls(List<UserAsset> assets);

		protected static void UpdateAssetTabControl<TUserAsset>(List<TUserAsset> userAssets, AssetTabControl assetTabControl)
			where TUserAsset : UserAsset
		{
			foreach (AssetRowControl rowHandler in assetTabControl.RowHandlers)
			{
				AbstractAsset asset = rowHandler.Asset;
				TUserAsset userAsset = userAssets.FirstOrDefault(a => a.AssetName == asset.AssetName && a.AssetType == asset.AssetType);
				if (userAsset != null)
				{
					asset.ImportValuesFromUserAsset(userAsset);

					rowHandler.UpdateGui();
				}
			}
		}
	}
}