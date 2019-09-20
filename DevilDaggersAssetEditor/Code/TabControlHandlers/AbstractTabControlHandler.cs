﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using DevilDaggersAssetEditor.Code.ExpanderControlHandlers;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public abstract class AbstractTabControlHandler
	{
		public abstract AbstractBinaryFileHandler FileHandler { get; }

		public virtual MenuItem CreateFileTypeMenuItem()
		{
			BinaryFileType binaryFileType = FileHandler.BinaryFileType;
			string fileName = binaryFileType.ToString().ToLower();

			MenuItem extractItem = new MenuItem { Header = $"Extract '{fileName}'" };
			MenuItem compressItem = new MenuItem { Header = $"Compress '{fileName}'", IsEnabled = binaryFileType == BinaryFileType.Audio };
			MenuItem openModFileItem = new MenuItem { Header = $"Open .{fileName} mod file", IsEnabled = binaryFileType == BinaryFileType.Audio };
			MenuItem saveModFileItem = new MenuItem { Header = $"Save .{fileName} mod file", IsEnabled = binaryFileType == BinaryFileType.Audio };

			extractItem.Click += (sender, e) => Extract_Click();
			compressItem.Click += (sender, e) => Compress_Click();
			openModFileItem.Click += (sender, e) =>
			{
				IEnumerable<GenericUserAsset> modFile = OpenModFile();
				if (modFile == null)
				{
					App.Instance.ShowMessage("Mod not loaded", "Could not parse mod file.");
					return;
				}
				UpdateExpanderControls(modFile);
			};
			saveModFileItem.Click += (sender, e) =>
			{
				List<AbstractAsset> assets = GetAssets();
				IEnumerable<GenericUserAsset> userAssets = CreateUserAssets(assets);
				SaveModFile(userAssets);
			};

			MenuItem fileTypeMenuItem = new MenuItem { Header = fileName, IsEnabled = binaryFileType != BinaryFileType.Particle };

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

		private void Extract_Click()
		{
			OpenFileDialog openDialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			using (CommonOpenFileDialog saveDialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Utils.DDFolder })
			{
				CommonFileDialogResult saveResult = saveDialog.ShowDialog();
				if (saveResult == CommonFileDialogResult.Ok)
					FileHandler.Extract(openDialog.FileName, saveDialog.FileName);
			}
		}

		private void Compress_Click()
		{
			if (!IsComplete())
			{
				MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (promptResult == MessageBoxResult.No)
					return;
			}

			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Path.Combine(Utils.DDFolder, FileHandler.BinaryFileType.GetSubfolderName()) };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			FileHandler.Compress(GetAssets(), dialog.FileName);
		}

		private void SaveModFile(IEnumerable<GenericUserAsset> assets)
		{
			string modFileExtension = FileHandler.BinaryFileType.ToString().ToLower();
			string modFileFilter = $"{FileHandler.BinaryFileType.ToString()} mod files (*.{modFileExtension})|*.{modFileExtension}";
			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = Utils.DDFolder, AddExtension = true, DefaultExt = modFileExtension, Filter = modFileFilter };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			JsonUtils.SerializeToFile(dialog.FileName, assets, true, Formatting.None);
		}

		private IEnumerable<GenericUserAsset> CreateUserAssets(List<AbstractAsset> assets)
		{
			foreach (AbstractAsset asset in assets)
				yield return asset.ToUserAsset();
		}

		private IEnumerable<GenericUserAsset> OpenModFile()
		{
			string modFileExtension = FileHandler.BinaryFileType.ToString().ToLower();
			string modFileFilter = $"{FileHandler.BinaryFileType.ToString()} mod files (*.{modFileExtension})|*.{modFileExtension}";
			OpenFileDialog dialog = new OpenFileDialog { InitialDirectory = Utils.DDFolder, AddExtension = true, DefaultExt = modFileExtension, Filter = modFileFilter };
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return null;

			return JsonUtils.TryDeserializeFromFile<IEnumerable<GenericUserAsset>>(dialog.FileName, true);
		}

		protected abstract void UpdateExpanderControls(IEnumerable<GenericUserAsset> assets);

		protected void UpdateExpanderControl<TUserAsset, TAsset, TAssetControl>(IEnumerable<TUserAsset> userAssets, AbstractExpanderControlHandler<TAsset, TAssetControl> expanderControlHandler) where TUserAsset : GenericUserAsset where TAsset : AbstractAsset where TAssetControl : UserControl
		{
			for (int i = 0; i < expanderControlHandler.Assets.Count; i++)
			{
				TAsset asset = expanderControlHandler.Assets[i];
				TUserAsset userAsset = userAssets.Where(a => a.AssetName == asset.AssetName).FirstOrDefault();
				if (userAsset != null)
				{
					asset.ImportValuesFromUserAsset(userAsset);

					expanderControlHandler.UpdateGUI(asset);
				}
			}
		}
	}
}