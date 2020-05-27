using DevilDaggersAssetCore.ModFiles;
using DevilDaggersAssetCore.User;
using JsonUtils;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.Code
{
	public sealed class ModHandler
	{
		private UserSettings Settings => UserHandler.Instance.settings;

		private static readonly Lazy<ModHandler> lazy = new Lazy<ModHandler>(() => new ModHandler());
		public static ModHandler Instance => lazy.Value;

		private ModHandler()
		{
		}

		public ModFile GetModFileFromPath(string path)
		{
			if (!JsonFileUtils.TryDeserializeFromFile(path, true, out ModFile modFile))
				App.Instance.ShowMessage("Mod not loaded", "Could not parse mod file.");

			if (modFile.HasRelativePaths)
			{
				App.Instance.ShowMessage("Specify base path", "This mod file uses relative paths. Please specify a base path.");
				using CommonOpenFileDialog basePathDialog = new CommonOpenFileDialog { IsFolderPicker = true };
				if (Settings.EnableAssetsRootFolder)
					basePathDialog.InitialDirectory = Settings.AssetsRootFolder;

				CommonFileDialogResult result = basePathDialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok)
					foreach (AbstractUserAsset asset in modFile.Assets)
						asset.EditorPath = Path.Combine(basePathDialog.FileName, asset.EditorPath);
			}

			UserHandler.Instance.cache.OpenedModFilePath = path;

			return modFile;
		}
	}
}