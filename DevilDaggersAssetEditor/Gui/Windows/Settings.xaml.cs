using DevilDaggersAssetEditor.Code.User;
using DevilDaggersCore.MemoryHandling;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DevilDaggersAssetEditor.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		public SettingsWindow()
		{
			InitializeComponent();

			LabelDevilDaggersRootFolder.Content = UserHandler.Instance.settings.DevilDaggersRootFolder;
			LabelModsRootFolder.Content = UserHandler.Instance.settings.ModsRootFolder;
			LabelAssetsRootFolder.Content = UserHandler.Instance.settings.AssetsRootFolder;

			Data.DataContext = UserHandler.Instance.settings;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Removes Exit button.
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			// Prevents Alt F4 from closing the window.
			if (!DialogResult.HasValue || !DialogResult.Value)
				e.Cancel = true;
		}

		private void BrowseDevilDaggersRootFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = UserHandler.Instance.settings.DevilDaggersRootFolder
			})
			{
				CommonFileDialogResult result = dialog.ShowDialog();

				if (result == CommonFileDialogResult.Ok)
					SetDevilDaggersRootFolder(dialog.FileName);
			}
		}

		private void BrowseModsRootFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = UserHandler.Instance.settings.DevilDaggersRootFolder
			})
			{
				CommonFileDialogResult result = dialog.ShowDialog();

				if (result == CommonFileDialogResult.Ok)
				{
					UserHandler.Instance.settings.ModsRootFolder = dialog.FileName;
					LabelModsRootFolder.Content = UserHandler.Instance.settings.ModsRootFolder;
				}
			}
		}

		private void BrowseAssetsRootFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = UserHandler.Instance.settings.DevilDaggersRootFolder
			})
			{
				CommonFileDialogResult result = dialog.ShowDialog();

				if (result == CommonFileDialogResult.Ok)
				{
					UserHandler.Instance.settings.AssetsRootFolder = dialog.FileName;
					LabelAssetsRootFolder.Content = UserHandler.Instance.settings.AssetsRootFolder;
				}
			}
		}

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			Process process = ProcessUtils.GetDevilDaggersProcess();
			if (process != null)
			{
				SetDevilDaggersRootFolder(Path.GetDirectoryName(process.MainModule.FileName));
				return;
			}

			App.Instance.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void SetDevilDaggersRootFolder(string path)
		{
			UserHandler.Instance.settings.DevilDaggersRootFolder = path;
			LabelDevilDaggersRootFolder.Content = UserHandler.Instance.settings.DevilDaggersRootFolder;
		}
	}
}