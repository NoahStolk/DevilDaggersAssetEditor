using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code.Web;
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
			if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue && !NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
			{
				UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
				updateRecommendedWindow.ShowDialog();
			}
		}
	}
}