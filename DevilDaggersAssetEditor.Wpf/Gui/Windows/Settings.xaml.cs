using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
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

			CheckBoxOpenModFolderAfterExtracting.IsChecked = UserHandler.Instance.Settings.OpenModFolderAfterExtracting;

			TextBoxTextureSizeLimit.Text = UserHandler.Instance.Settings.TextureSizeLimit.ToString();
		}

		private void Browse(Label label)
		{
			VistaFolderBrowserDialog dialog = new();
			dialog.OpenDirectory(true, label.Content.ToString());

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
			if (!string.IsNullOrWhiteSpace(process?.MainModule?.FileName))
				LabelDevilDaggersRootFolder.Content = Path.GetDirectoryName(process.MainModule.FileName);
			else
				App.Instance.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void TextBoxTextureSizeLimit_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBoxTextureSizeLimit.Background = uint.TryParse(TextBoxTextureSizeLimit.Text, out uint res) && res > 0 ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorText"];
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			UserHandler.Instance.Settings.AssetsRootFolder = LabelAssetsRootFolder.Content.ToString() ?? UserSettings.PathDefault;
			UserHandler.Instance.Settings.DevilDaggersRootFolder = LabelDevilDaggersRootFolder.Content.ToString() ?? UserSettings.PathDefault;
			UserHandler.Instance.Settings.ModsRootFolder = LabelModsRootFolder.Content.ToString() ?? UserSettings.PathDefault;

			UserHandler.Instance.Settings.EnableAssetsRootFolder = CheckBoxAssetsRootFolder.IsChecked();
			UserHandler.Instance.Settings.EnableDevilDaggersRootFolder = CheckBoxDevilDaggersRootFolder.IsChecked();
			UserHandler.Instance.Settings.EnableModsRootFolder = CheckBoxModsRootFolder.IsChecked();

			UserHandler.Instance.Settings.OpenModFolderAfterExtracting = CheckBoxOpenModFolderAfterExtracting.IsChecked();

			if (uint.TryParse(TextBoxTextureSizeLimit.Text, out uint res) && res > 0)
				UserHandler.Instance.Settings.TextureSizeLimit = res;

			DialogResult = true;
		}
	}
}
