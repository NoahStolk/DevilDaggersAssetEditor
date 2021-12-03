using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using System;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows;

public partial class ModManagerWindow : Window
{
	private readonly DownloadModsControl _downloadModsControl;

	public ModManagerWindow()
	{
		InitializeComponent();

		TabControl.SelectedIndex = Math.Clamp(UserHandler.Instance.Cache.ModManagerActiveTabIndex, 0, 1);

		_downloadModsControl = new(this);
		DownloadModsTabItem.Content = _downloadModsControl;
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		_downloadModsControl.SaveCache();

		UserHandler.Instance.Cache.ModManagerActiveTabIndex = TabControl.SelectedIndex;
	}
}