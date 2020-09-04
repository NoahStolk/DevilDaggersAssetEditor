using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Code;
using DevilDaggersCore.Utils;
using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
#pragma warning disable IDE1006
#pragma warning disable SA1310 // Field names should not contain underscore
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
#pragma warning restore IDE1006
#pragma warning restore SA1310 // Field names should not contain underscore

		public SettingsWindow()
		{
			InitializeComponent();

			TextBoxTextureSizeLimit.TextChanged += TextBoxTextureSizeLimit_TextChanged;

			LabelAssetsRootFolder.Content = UserHandler.Instance.Settings.AssetsRootFolder;
			LabelDevilDaggersRootFolder.Content = UserHandler.Instance.Settings.DevilDaggersRootFolder;
			LabelModsRootFolder.Content = UserHandler.Instance.Settings.ModsRootFolder;

			CheckBoxAssetsRootFolder.IsChecked = UserHandler.Instance.Settings.EnableAssetsRootFolder;
			CheckBoxDevilDaggersRootFolder.IsChecked = UserHandler.Instance.Settings.EnableDevilDaggersRootFolder;
			CheckBoxModsRootFolder.IsChecked = UserHandler.Instance.Settings.EnableModsRootFolder;

			CheckBoxCreateModFileWhenExtracting.IsChecked = UserHandler.Instance.Settings.CreateModFileWhenExtracting;
			CheckBoxOpenModFolderAfterExtracting.IsChecked = UserHandler.Instance.Settings.OpenModFolderAfterExtracting;

			TextBoxTextureSizeLimit.Text = UserHandler.Instance.Settings.TextureSizeLimit.ToString(CultureInfo.InvariantCulture);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Removes Exit button.
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			NativeMethods.SetWindowLong(hwnd, GWL_STYLE, NativeMethods.GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			// Prevents Alt F4 from closing the window.
			if (!DialogResult.HasValue || !DialogResult.Value)
				e.Cancel = true;
		}

		private void Browse(Label label)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			string? initDir = label.Content.ToString();
			if (!string.IsNullOrEmpty(initDir) && Directory.Exists(initDir))
				dialog.SelectedPath = initDir;

			if (dialog.ShowDialog() == true)
				label.Content = dialog.SelectedPath;

			Focus();
		}

		private void BrowseDevilDaggersRootFolderButton_Click(object sender, RoutedEventArgs e)
			=> Browse(LabelDevilDaggersRootFolder);

		private void BrowseModsRootFolderButton_Click(object sender, RoutedEventArgs e)
			=> Browse(LabelModsRootFolder);

		private void BrowseAssetsRootFolderButton_Click(object sender, RoutedEventArgs e)
			=> Browse(LabelAssetsRootFolder);

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			Process? process = ProcessUtils.GetDevilDaggersProcess();
			if (process != null)
				LabelDevilDaggersRootFolder.Content = Path.GetDirectoryName(process.MainModule.FileName);
			else
				App.Instance.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void TextBoxTextureSizeLimit_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBoxTextureSizeLimit.Background = uint.TryParse(TextBoxTextureSizeLimit.Text, out uint res) && res > 0 ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 127, 127));
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			UserHandler.Instance.Settings.AssetsRootFolder = LabelAssetsRootFolder.Content.ToString() ?? UserSettings.PathDefault;
			UserHandler.Instance.Settings.DevilDaggersRootFolder = LabelDevilDaggersRootFolder.Content.ToString() ?? UserSettings.PathDefault;
			UserHandler.Instance.Settings.ModsRootFolder = LabelModsRootFolder.Content.ToString() ?? UserSettings.PathDefault;

			UserHandler.Instance.Settings.EnableAssetsRootFolder = CheckBoxAssetsRootFolder.IsChecked.Value;
			UserHandler.Instance.Settings.EnableDevilDaggersRootFolder = CheckBoxDevilDaggersRootFolder.IsChecked.Value;
			UserHandler.Instance.Settings.EnableModsRootFolder = CheckBoxModsRootFolder.IsChecked.Value;

			UserHandler.Instance.Settings.CreateModFileWhenExtracting = CheckBoxCreateModFileWhenExtracting.IsChecked.Value;
			UserHandler.Instance.Settings.OpenModFolderAfterExtracting = CheckBoxOpenModFolderAfterExtracting.IsChecked.Value;

			if (uint.TryParse(TextBoxTextureSizeLimit.Text, out uint res) && res > 0)
				UserHandler.Instance.Settings.TextureSizeLimit = res;

			DialogResult = true;
		}
	}
}