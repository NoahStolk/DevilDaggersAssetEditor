using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Clients;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersCore.Wpf.Windows;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class ModPreviewControl : UserControl
	{
		private readonly ModManagerWindow _modManagerWindow;

		private string? _selectedModName;

		public ModPreviewControl(ModManagerWindow modManagerWindow)
		{
			InitializeComponent();

			_modManagerWindow = modManagerWindow;
		}

		public void Update(Mod? mod)
		{
			ScrollViewerDescription.ScrollToTop();
			ScrollViewerBinaries.ScrollToTop();
			ScrollViewerScreenshots.ScrollToTop();

			PreviewBinariesList.Children.Clear();
			PreviewScreenshotsList.Children.Clear();

			_selectedModName = mod?.Name;
			DownloadModButton.IsEnabled = mod?.ModArchive != null;
			PreviewName.Content = _selectedModName ?? "No mod selected";
			PreviewDescription.Text = mod?.HtmlDescription;

			BinariesContainer.Visibility = mod == null ? Visibility.Collapsed : Visibility.Visible;
			DescriptionContainer.Visibility = string.IsNullOrWhiteSpace(PreviewDescription.Text) ? Visibility.Collapsed : Visibility.Visible;
			ScreenshotsContainer.Visibility = mod?.ScreenshotFileNames.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

			if (mod?.ModArchive != null)
			{
				foreach (ModBinary binary in mod.ModArchive.Binaries)
					PreviewBinariesList.Children.Add(new TextBlock { Text = binary.Name, Margin = new Thickness(0, 0, 0, 8) });

				foreach (string screenshotFileName in mod.ScreenshotFileNames)
				{
					PreviewScreenshotsList.Children.Add(new Image
					{
						Stretch = Stretch.Fill,
						HorizontalAlignment = HorizontalAlignment.Left,
						Source = new BitmapImage(new Uri($"https://devildaggers.info/mod-screenshots/{mod.Name}/{screenshotFileName}")),
					});
				}
			}
		}

		private async void DownloadModButton_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedModName == null)
				return;

			string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");

			ModArchive? archive = NetworkHandler.Instance.Mods.Find(m => m.Name == _selectedModName)?.ModArchive;
			if (archive != null)
			{
				foreach (ModBinary binary in archive.Binaries)
				{
					if (File.Exists(Path.Combine(modsDirectory, binary.Name)))
					{
						ConfirmWindow window = new("File already exists", $"The mod '{_selectedModName}' contains a binary called '{binary.Name}'. A file with the same name already exists in the mods directory. Are you sure you want to overwrite it by downloading the '{_selectedModName}' mod?", false);
						window.ShowDialog();

						if (window.IsConfirmed != true)
							return;
					}
				}
			}

			DownloadAndInstallModWindow downloadingWindow = new();
			downloadingWindow.Show();
			await downloadingWindow.DownloadAndInstall(modsDirectory, _selectedModName);

			_modManagerWindow.ManageModsControl.PopulateModFilesList();
		}
	}
}
