using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Caching;
using DevilDaggersAssetEditor.Wpf.Clients;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersCore.Wpf.Windows;
using HTMLConverter;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls;

public partial class ModPreviewControl : UserControl
{
	private readonly ModManagerWindow _modManagerWindow;

	private GetModDdae? _selectedMod;

	private int _screenshotIndex;

	public ModPreviewControl(ModManagerWindow modManagerWindow)
	{
		InitializeComponent();

		_modManagerWindow = modManagerWindow;
	}

	public void Update(GetModDdae? mod)
	{
		_screenshotIndex = 0;

		ScrollViewerDescription.ScrollToTop();
		ScrollViewerBinaries.ScrollToTop();

		PreviewBinariesList.Children.Clear();

		_selectedMod = mod;
		DownloadModButton.IsEnabled = mod?.ModArchive != null;
		PreviewName.Content = _selectedMod?.Name ?? "No mod selected";
		PreviewDescription.Text = string.IsNullOrWhiteSpace(mod?.HtmlDescription) ? null : HtmlToXamlConverter.ConvertHtmlToXaml(mod?.HtmlDescription, false);

		BinariesContainer.Visibility = mod == null ? Visibility.Collapsed : Visibility.Visible;
		DescriptionContainer.Visibility = string.IsNullOrWhiteSpace(mod?.HtmlDescription) ? Visibility.Collapsed : Visibility.Visible;
		ScreenshotsContainer.Visibility = mod?.ScreenshotFileNames.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

		if (mod?.ModArchive != null)
		{
			foreach (GetModBinaryDdae binary in mod.ModArchive.Binaries)
				PreviewBinariesList.Children.Add(new TextBlock { Text = binary.Name, Margin = new Thickness(0, 0, 0, 8) });
		}

		UpdateScreenshotUi();
	}

	private void UpdateScreenshotUi()
	{
		if (_selectedMod == null || _selectedMod.ScreenshotFileNames.Count == 0)
		{
			Screenshot.Source = null;
			ScreenshotLabel.Content = null;
		}
		else
		{
			Screenshot.Source = ModScreenshotCache.Instance.GetScreenshot(_selectedMod.Name, _selectedMod.ScreenshotFileNames[_screenshotIndex]);
			ScreenshotLabel.Content = $"Screenshot {_screenshotIndex + 1} of {_selectedMod.ScreenshotFileNames.Count}";
		}
	}

	private void FirstScreenshot_Click(object sender, RoutedEventArgs e)
	{
		_screenshotIndex = 0;
		UpdateScreenshotUi();
	}

	private void PreviousScreenshot_Click(object sender, RoutedEventArgs e)
	{
		_screenshotIndex = Math.Max(0, _screenshotIndex - 1);
		UpdateScreenshotUi();
	}

	private void NextScreenshot_Click(object sender, RoutedEventArgs e)
	{
		_screenshotIndex = Math.Min((_selectedMod?.ScreenshotFileNames.Count - 1) ?? 0, _screenshotIndex + 1);
		UpdateScreenshotUi();
	}

	private void LastScreenshot_Click(object sender, RoutedEventArgs e)
	{
		_screenshotIndex = (_selectedMod?.ScreenshotFileNames.Count - 1) ?? 0;
		UpdateScreenshotUi();
	}

	private async void DownloadModButton_Click(object sender, RoutedEventArgs e)
	{
		if (_selectedMod == null)
			return;

		string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");

		GetModArchiveDdae? archive = NetworkHandler.Instance.Mods.Find(m => m.Name == _selectedMod?.Name)?.ModArchive;
		if (archive != null)
		{
			foreach (GetModBinaryDdae binary in archive.Binaries.Where(b => File.Exists(Path.Combine(modsDirectory, b.Name))))
			{
				ConfirmWindow window = new("File already exists", $"The mod '{_selectedMod.Name}' contains a binary called '{binary.Name}'. A file with the same name already exists in the mods directory. Are you sure you want to overwrite it by downloading the '{_selectedMod.Name}' mod?", false);
				window.ShowDialog();

				if (window.IsConfirmed != true)
					return;
			}
		}

		DownloadAndInstallModWindow downloadingWindow = new();
		downloadingWindow.Show();
		await downloadingWindow.DownloadAndInstall(modsDirectory, _selectedMod.Name);

		_modManagerWindow.ManageModsControl.UpdateUi();
	}
}
