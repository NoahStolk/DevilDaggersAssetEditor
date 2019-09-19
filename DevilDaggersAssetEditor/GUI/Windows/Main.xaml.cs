using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code.Web;
using System.ComponentModel;
using System.Windows;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public BinaryFileName activeBinaryFileName;

		public MainWindow()
		{
			InitializeComponent();

			App.Instance.MainWindow = this;
			App.Instance.UpdateMainWindowTitle();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += (object checkVersionSender, DoWorkEventArgs checkVersionE) =>
			{
				NetworkHandler.Instance.RetrieveVersion();
			};
			worker.RunWorkerCompleted += (object checkVersionSender, RunWorkerCompletedEventArgs echeckVersionE) =>
			{
				if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue && !NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
					updateRecommendedWindow.ShowDialog();
				}
			};
			worker.RunWorkerAsync();
		}
	}
}