using DevilDaggersAssetEditor.User;
using System;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ModManagerWindow : Window
	{
		public ModManagerWindow()
		{
			InitializeComponent();

			TabControl.SelectedIndex = Math.Clamp(UserHandler.Instance.Cache.ModManagerActiveTabIndex, 0, 1);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			DownloadModsControl.SaveCache();

			UserHandler.Instance.Cache.ModManagerActiveTabIndex = TabControl.SelectedIndex;
		}
	}
}
