using DevilDaggersCore.Tools;
using System;
using System.ComponentModel;
using System.Windows;

namespace DevilDaggersAssetEditor.Gui.Windows
{
	public partial class CheckingForUpdatesWindow : Window
	{
		public CheckingForUpdatesWindow()
		{
			InitializeComponent();

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (sender, e) => VersionHandler.Instance.GetOnlineVersion(App.ApplicationName, App.LocalVersion);
			thread.RunWorkerCompleted += (sender, e) => Close();

			thread.RunWorkerAsync();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			VersionHandler.Instance.VersionResult.Exception = new Exception("Canceled by user");
			Close();
		}
	}
}