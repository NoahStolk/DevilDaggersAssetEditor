using DevilDaggersAssetEditor.Code.Network;
using DevilDaggersAssetEditor.Code.User;
using DevilDaggersCore.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		private readonly List<Point> tabControlSizes = new List<Point>
		{
			new Point(3840, 2160),
			new Point(2560, 1440),
			new Point(2048, 1152),
			new Point(1920, 1200),
			new Point(1920, 1080),
			new Point(1680, 1050),
			new Point(1440, 900),
			new Point(1366, 768)
		};

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
				NetworkHandler.Instance.VersionResult = VersionHandler.Instance.GetOnlineVersion(App.ApplicationName, App.LocalVersion);
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

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			for (int i = 0; i < tabControlSizes.Count; i++)
			{
				Point size = tabControlSizes[i];
				if (i == tabControlSizes.Count - 1 || (ActualWidth >= size.X && ActualHeight >= size.Y))
				{
					TabControl.Width = size.X - 8;
					TabControl.Height = size.Y - 64;
					break;
				}
			}
		}
	}
}