using DevilDaggersAssetCore.User;
using DevilDaggersCore.Processes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

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

		private UserSettings Settings => UserHandler.Instance.settings;

		public SettingsWindow()
		{
			InitializeComponent();

			TextBoxTextureSizeLimit.TextChanged += TextBoxTextureSizeLimit_TextChanged;

			LabelAssetsRootFolder.Content = Settings.AssetsRootFolder;
			LabelDevilDaggersRootFolder.Content = Settings.DevilDaggersRootFolder;
			LabelModsRootFolder.Content = Settings.ModsRootFolder;

			CheckBoxAssetsRootFolder.IsChecked = Settings.EnableAssetsRootFolder;
			CheckBoxDevilDaggersRootFolder.IsChecked = Settings.EnableDevilDaggersRootFolder;
			CheckBoxModsRootFolder.IsChecked = Settings.EnableModsRootFolder;

			CheckBoxCreateModFileWhenExtracting.IsChecked = Settings.CreateModFileWhenExtracting;
			CheckBoxOpenModFolderAfterExtracting.IsChecked = Settings.OpenModFolderAfterExtracting;

			TextBoxTextureSizeLimit.Text = Settings.TextureSizeLimit.ToString();
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

		private void TextBoxTextureSizeLimit_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBoxTextureSizeLimit.Background = uint.TryParse(TextBoxTextureSizeLimit.Text, out uint res) && res > 0 ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 127, 127));
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.AssetsRootFolder = LabelAssetsRootFolder.Content.ToString();
			Settings.DevilDaggersRootFolder = LabelDevilDaggersRootFolder.Content.ToString();
			Settings.ModsRootFolder = LabelModsRootFolder.Content.ToString();

			Settings.EnableAssetsRootFolder = CheckBoxAssetsRootFolder.IsChecked.Value;
			Settings.EnableDevilDaggersRootFolder = CheckBoxDevilDaggersRootFolder.IsChecked.Value;
			Settings.EnableModsRootFolder = CheckBoxModsRootFolder.IsChecked.Value;

			Settings.CreateModFileWhenExtracting = CheckBoxCreateModFileWhenExtracting.IsChecked.Value;
			Settings.OpenModFolderAfterExtracting = CheckBoxOpenModFolderAfterExtracting.IsChecked.Value;

			if (uint.TryParse(TextBoxTextureSizeLimit.Text, out uint res) && res > 0)
				Settings.TextureSizeLimit = res;

			DialogResult = true;
		}
	}
}