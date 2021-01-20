using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class ImportDirectoryControl : UserControl
	{
		private string _directory = UserHandler.Instance.Settings.AssetsRootFolder;

		public ImportDirectoryControl(string header, AssetType assetType, AssetTabControl assetTabControl)
		{
			InitializeComponent();
			UpdateGui();

			AssetType = assetType;
			AssetTabControl = assetTabControl;

			Header.Content = header;
		}

		public string Directory => _directory;

		public AssetType AssetType { get; }
		public AssetTabControl AssetTabControl { get; }

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _directory);

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
			=> _directory = TextBox.Text;

		private void UpdateGui()
			=> TextBox.Text = _directory;

		private void SetPath(ref string path)
		{
			if (TrySetPath(out string selectedPath))
			{
				path = selectedPath;
				UpdateGui();
			}
		}

		private static bool TrySetPath(out string selectedPath)
		{
			VistaFolderBrowserDialog dialog = new();
			dialog.OpenAssetsRootFolder();

			if (dialog.ShowDialog() != true)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = dialog.SelectedPath;
			return true;
		}

		private void CheckBoxEnable_Changed(object sender, RoutedEventArgs e)
		{
			bool isChecked = CheckBoxEnable.IsChecked();

			if (TextBox != null)
				TextBox.IsEnabled = isChecked;
			if (ButtonBrowse != null)
				ButtonBrowse.IsEnabled = isChecked;
			if (CheckBoxAllDirectories != null)
				CheckBoxAllDirectories.IsEnabled = isChecked;

			Main.Background = ColorUtils.ThemeColors[isChecked ? "Gray3" : "Gray2"];
		}
	}
}
