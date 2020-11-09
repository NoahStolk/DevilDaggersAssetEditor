using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class ImportDirectoryControl : UserControl
	{
		private string _directory = UserHandler.Instance.Settings.AssetsRootFolder;

		public ImportDirectoryControl(string header)
		{
			InitializeComponent();
			UpdateGui();

			Header.Content = header;
		}

		public string Directory => _directory;

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
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.OpenAssetsRootFolder();

			if (dialog.ShowDialog() != true)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = dialog.SelectedPath;
			return true;
		}
	}
}