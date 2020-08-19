using DevilDaggersCore.Utils;
using System.Windows;
using System.Windows.Navigation;

namespace DevilDaggersAssetEditor.Gui.Windows
{
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();

			VersionLabel.Content = $"Version {App.LocalVersion}";
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			ProcessUtils.OpenUrl(e.Uri.AbsoluteUri);
			e.Handled = true;
		}
	}
}