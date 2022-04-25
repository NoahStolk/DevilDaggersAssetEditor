using DevilDaggersAssetEditor.Mods;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using Ookii.Dialogs.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls;

public partial class ImportDirectoryControl : UserControl
{
	private readonly Action<bool> _updateCheckBoxCache;
	private readonly Action<bool> _updateCheckBoxAllDirectoriesCache;

	private string _directory = UserHandler.Instance.Settings.AssetsRootFolder;

	public ImportDirectoryControl(string header, AssetType assetType, AssetTabControl assetTabControl, bool checkBoxState, bool checkBoxAllDirectoriesState, Action<bool> updateCheckBoxCache, Action<bool> updateCheckBoxAllDirectoriesCache)
	{
		InitializeComponent();
		UpdateGui();

		_updateCheckBoxCache = updateCheckBoxCache;
		_updateCheckBoxAllDirectoriesCache = updateCheckBoxAllDirectoriesCache;

		AssetType = assetType;
		AssetTabControl = assetTabControl;
		Header.Content = header;

		CheckBoxEnable.IsChecked = checkBoxState;
		CheckBoxAllDirectories.IsChecked = checkBoxAllDirectoriesState;
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
		_updateCheckBoxCache(isChecked);

		if (TextBox != null)
			TextBox.IsEnabled = isChecked;
		if (ButtonBrowse != null)
			ButtonBrowse.IsEnabled = isChecked;
		if (CheckBoxAllDirectories != null)
			CheckBoxAllDirectories.IsEnabled = isChecked;

		Main.Background = ColorUtils.ThemeColors[isChecked ? "Gray3" : "Gray2"];
	}

	private void CheckBoxAllDirectories_Changed(object sender, RoutedEventArgs e)
	{
		bool isChecked = CheckBoxAllDirectories.IsChecked();
		_updateCheckBoxAllDirectoriesCache(isChecked);
	}
}
