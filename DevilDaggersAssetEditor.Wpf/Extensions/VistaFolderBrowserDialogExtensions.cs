using DevilDaggersAssetEditor.User;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Extensions
{
	public static class VistaFolderBrowserDialogExtensions
	{
		public static void OpenAssetsRootFolder(this VistaFolderBrowserDialog folderDialog)
			=> folderDialog.OpenDirectory(UserHandler.Instance.Settings.EnableAssetsRootFolder, UserHandler.Instance.Settings.AssetsRootFolder);

		public static void OpenDirectory(this VistaFolderBrowserDialog folderDialog, bool condition, string? path)
		{
			if (condition && !string.IsNullOrEmpty(path) && Directory.Exists(path))
				folderDialog.SelectedPath = $"{path}\\";
		}
	}
}