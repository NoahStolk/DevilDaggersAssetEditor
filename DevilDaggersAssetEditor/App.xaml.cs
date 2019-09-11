using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor
{
	public partial class App : Application
	{
		public App()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			Dispatcher.UnhandledException += OnDispatcherUnhandledException;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message, "FATAL ERROR");
		}
	}
}