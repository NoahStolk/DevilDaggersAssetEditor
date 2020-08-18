using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Json;
using DevilDaggersAssetCore.ModFiles;
using DevilDaggersAssetCore.User;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.Code
{
	public sealed class ModHandler
	{
		private static readonly Lazy<ModHandler> lazy = new Lazy<ModHandler>(() => new ModHandler());

		private ModHandler()
		{
		}

		public static ModHandler Instance => lazy.Value;

		public ModFile GetModFileFromPath(string path, BinaryFileType binaryFileType)
		{
			if (!JsonFileUtils.TryDeserializeFromFile(path, true, out ModFile modFile))
				App.Instance.ShowMessage("Mod not loaded", "Could not parse mod file.");

			if (modFile.HasRelativePaths)
			{
				App.Instance.ShowMessage("Specify base path", "This mod file uses relative paths. Please specify a base path.");
				VistaFolderBrowserDialog basePathDialog = new VistaFolderBrowserDialog();
				if (UserHandler.Instance.settings.EnableAssetsRootFolder && Directory.Exists(UserHandler.Instance.settings.AssetsRootFolder))
					basePathDialog.SelectedPath = UserHandler.Instance.settings.AssetsRootFolder;

				if (basePathDialog.ShowDialog() == true)
				{
					foreach (AbstractUserAsset asset in modFile.Assets)
						asset.EditorPath = Path.Combine(basePathDialog.SelectedPath, asset.EditorPath);
				}
			}

			switch (binaryFileType)
			{
				case BinaryFileType.Audio: UserHandler.Instance.cache.OpenedAudioModFilePath = path; break;
				case BinaryFileType.Core: UserHandler.Instance.cache.OpenedCoreModFilePath = path; break;
				case BinaryFileType.Dd: UserHandler.Instance.cache.OpenedDdModFilePath = path; break;
				case BinaryFileType.Particle: UserHandler.Instance.cache.OpenedParticleModFilePath = path; break;
				default: throw new NotImplementedException($"{nameof(BinaryFileType)} {binaryFileType} not implemented in {nameof(GetModFileFromPath)} method.");
			}

			return modFile;
		}
	}
}