using DevilDaggersAssetEditor.Code.Web;
using System.ComponentModel;
using System.Windows;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class CheckingForUpdatesWindow : Window
	{
		public CheckingForUpdatesWindow()
		{
			InitializeComponent();

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += Thread_DoWork;
			thread.RunWorkerCompleted += Thread_RunWorkerCompleted;

			thread.RunWorkerAsync();
		}

		private void Thread_DoWork(object sender, DoWorkEventArgs e)
		{
			NetworkHandler.Instance.RetrieveVersion();
		}

		private void Thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			NetworkHandler.Instance.VersionResult = new VersionResult(null, string.Empty, "Canceled by user");
			Close();
		}
	}
}