using DevilDaggersAssetCore.User;
using DevilDaggersCore.Processes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DevilDaggersAssetEditor.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
#pragma warning disable IDE1006
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
#pragma warning restore IDE1006
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private UserSettings settings => UserHandler.Instance.settings;

		public SettingsWindow()
		{
			InitializeComponent();

			LabelAssetsRootFolder.Content = settings.AssetsRootFolder;
			LabelDevilDaggersRootFolder.Content = settings.DevilDaggersRootFolder;
			LabelModsRootFolder.Content = settings.ModsRootFolder;

			CheckBoxAssetsRootFolder.IsChecked = settings.EnableAssetsRootFolder;
			CheckBoxDevilDaggersRootFolder.IsChecked = settings.EnableDevilDaggersRootFolder;
			CheckBoxModsRootFolder.IsChecked = settings.EnableModsRootFolder;

			CheckBoxCreateModFileWhenExtracting.IsChecked = settings.CreateModFileWhenExtracting;
			CheckBoxOpenModFolderAfterExtracting.IsChecked = settings.OpenModFolderAfterExtracting;
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
			using CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = LabelDevilDaggersRootFolder.Content.ToString() };
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				LabelDevilDaggersRootFolder.Content = dialog.FileName;

			Focus();
		}

		private void BrowseModsRootFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = LabelModsRootFolder.Content.ToString() };
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				LabelModsRootFolder.Content = dialog.FileName;

			Focus();
		}

		private void BrowseAssetsRootFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = LabelAssetsRootFolder.Content.ToString() };
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				LabelAssetsRootFolder.Content = dialog.FileName;

			Focus();
		}

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			Process process = ProcessUtils.GetDevilDaggersProcess();
			if (process != null)
				LabelDevilDaggersRootFolder.Content = process.MainModule.FileName;
			else
				App.Instance.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			settings.AssetsRootFolder = LabelAssetsRootFolder.Content.ToString();
			settings.DevilDaggersRootFolder = LabelDevilDaggersRootFolder.Content.ToString();
			settings.ModsRootFolder = LabelModsRootFolder.Content.ToString();

			settings.EnableAssetsRootFolder = CheckBoxAssetsRootFolder.IsChecked.Value;
			settings.EnableDevilDaggersRootFolder = CheckBoxDevilDaggersRootFolder.IsChecked.Value;
			settings.EnableModsRootFolder = CheckBoxModsRootFolder.IsChecked.Value;

			settings.CreateModFileWhenExtracting = CheckBoxCreateModFileWhenExtracting.IsChecked.Value;
			settings.OpenModFolderAfterExtracting = CheckBoxOpenModFolderAfterExtracting.IsChecked.Value;

			DialogResult = true;
		}
	}
}