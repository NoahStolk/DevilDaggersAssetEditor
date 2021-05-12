using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Wpf.ModFiles
{
	public sealed class ModFileHandler
	{
		private bool _hasUnsavedChanges;

		private static readonly Lazy<ModFileHandler> _lazy = new(() => new());

		private ModFileHandler()
		{
		}

		public static ModFileHandler Instance => _lazy.Value;

		public List<UserAsset> ModFile { get; set; } = new();

		public bool HasUnsavedChanges
		{
			get => _hasUnsavedChanges;
			set
			{
				_hasUnsavedChanges = value;
				App.Instance.UpdateMainWindowTitle();
			}
		}

		public string ModFileName { get; private set; } = "(new mod)";
		public string ModFileLocation { get; private set; } = string.Empty;

		public void UpdateModFileState(string fileLocation)
		{
			UserHandler.Instance.Cache.OpenedModFilePath = fileLocation;

			HasUnsavedChanges = false;

			ModFileName = fileLocation.Length == 0 ? "(new mod)" : Path.GetFileNameWithoutExtension(fileLocation);
			ModFileLocation = fileLocation;

			App.Instance.UpdateMainWindowTitle();
		}

		/// <summary>
		/// Asks the user to save the file before proceeding.
		/// </summary>
		/// <returns><see langword="true"/> if the following action should be cancelled.</returns>
		public bool ProceedWithUnsavedChanges()
		{
			if (!HasUnsavedChanges)
				return false;

			ConfirmWindow confirmWindow = new("Save changes?", "The current mod has unsaved changes. Save before proceeding?", false);
			confirmWindow.ShowDialog();

			if (confirmWindow.IsConfirmed == true)
				FileSave();

			return confirmWindow.IsConfirmed == null;
		}

		public void FileOpen(string path)
		{
			List<UserAsset>? modFile = JsonFileUtils.TryDeserializeFromFile<List<UserAsset>>(path, true);
			if (modFile == null)
				return;

			UpdateModFileState(path);
			App.Instance.UpdateMainWindowTitle();

			ModFile = modFile;
		}

		public void FileSave()
		{
			if (File.Exists(ModFileLocation))
			{
				SaveAssets();

				JsonFileUtils.SerializeToFile(ModFileLocation, ModFile, true);
				HasUnsavedChanges = false;
			}
			else
			{
				FileSaveAs();
			}
		}

		public void FileSaveAs()
		{
			SaveAssets();

			SaveFileDialog dialog = new() { Filter = GuiUtils.ModFileFilter };
			dialog.OpenModsRootFolder();

			bool? result = dialog.ShowDialog();
			if (result == true)
			{
				ModFileLocation = dialog.FileName;

				JsonFileUtils.SerializeToFile(ModFileLocation, ModFile, true);
				UpdateModFileState(dialog.FileName);
			}
		}

		private void SaveAssets()
		{
			List<AbstractAsset> assets = App.Instance.MainWindow!.AssetTabControls.SelectMany(atc => atc.GetAssets()).ToList();

			ModFile.Clear();
			foreach (AbstractAsset asset in assets)
				ModFile.Add(asset.ToUserAsset());
		}
	}
}
