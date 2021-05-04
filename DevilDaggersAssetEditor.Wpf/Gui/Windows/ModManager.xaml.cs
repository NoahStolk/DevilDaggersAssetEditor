using System;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ModManagerWindow : Window
	{
		public ModManagerWindow()
		{
			InitializeComponent();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			DownloadModsControl.SaveCache();
		}
	}
}
