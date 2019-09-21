using DevilDaggersAssetEditor.Code.User;
using DevilDaggersAssetEditor.Code.Web;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			if (File.Exists(UserSettings.FileName))
				using (StreamReader sr = new StreamReader(File.OpenRead(UserSettings.FileName)))
					UserHandler.Instance.settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());

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