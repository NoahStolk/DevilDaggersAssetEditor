using DevilDaggersAssetEditor.User;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Extensions;

public static class DialogExtensions
{
	public static void OpenDevilDaggersRootFolder(this FileDialog folderDialog)
		=> folderDialog.OpenDirectory(UserHandler.Instance.Settings.EnableDevilDaggersRootFolder, UserHandler.Instance.Settings.DevilDaggersRootFolder);

	public static void OpenDevilDaggersModsFolder(this FileDialog folderDialog)
		=> folderDialog.OpenDirectory(UserHandler.Instance.Settings.EnableDevilDaggersRootFolder, Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods"));

	public static void OpenModsRootFolder(this FileDialog folderDialog)
		=> folderDialog.OpenDirectory(UserHandler.Instance.Settings.EnableModsRootFolder, UserHandler.Instance.Settings.ModsRootFolder);

	public static void OpenAssetsRootFolder(this VistaFolderBrowserDialog folderDialog)
		=> folderDialog.OpenDirectory(UserHandler.Instance.Settings.EnableAssetsRootFolder, UserHandler.Instance.Settings.AssetsRootFolder);

	public static void OpenDirectory(this VistaFolderBrowserDialog folderDialog, bool condition, string? path)
	{
		if (condition && !string.IsNullOrEmpty(path) && Directory.Exists(path))
			folderDialog.SelectedPath = $"{path}\\";
	}

	public static void OpenDirectory(this FileDialog folderDialog, bool condition, string? path)
	{
		if (condition && !string.IsNullOrEmpty(path) && Directory.Exists(path))
			folderDialog.InitialDirectory = path;
	}
}
