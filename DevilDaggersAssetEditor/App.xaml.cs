using DevilDaggersAssetCore;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Gui.Windows;
using DevilDaggersCore.Tools;
using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor
{
	public partial class App : Application
	{
		public static string ApplicationName => "DevilDaggersAssetEditor";
		public static string ApplicationDisplayName => "Devil Daggers Asset Editor";

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Assembly Assembly { get; private set; }
		public static Version LocalVersion { get; private set; }

		public static App Instance => (App)Current;
		public new MainWindow MainWindow { get; set; }

		private UserCache Cache => UserHandler.Instance.cache;

		public App()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			Assembly = Assembly.GetExecutingAssembly();
			LocalVersion = VersionHandler.GetLocalVersion(Assembly);
			Utils.GuiVersion = LocalVersion;

			Dispatcher.UnhandledException += OnDispatcherUnhandledException;

			XmlConfigurator.Configure();
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ShowError("Fatal error", "An unhandled exception occurred in the main thread.", e.Exception);
			e.Handled = true;

			Current.Shutdown();
		}

		public void UpdateMainWindowTitle()
		{
			Dispatcher.Invoke(() =>
			{
				MainWindow.Title = $"{ApplicationDisplayName} {LocalVersion}";
			});
		}

		/// <summary>
		/// Shows the error using the <see cref="ErrorWindow" /> and logs the error message (and <see cref="Exception" /> if there is one).
		/// </summary>
		public void ShowError(string title, string message, Exception ex = null)
		{
			if (ex != null)
				Log.Error(message, ex);
			else
				Log.Error(message);

			Dispatcher.Invoke(() =>
			{
				ErrorWindow errorWindow = new ErrorWindow(title, message, ex);
				errorWindow.ShowDialog();
			});
		}

		public void ShowMessage(string title, string message)
		{
			Dispatcher.Invoke(() =>
			{
				MessageWindow messageWindow = new MessageWindow(title, message);
				messageWindow.ShowDialog();
			});
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			Cache.ActiveTabIndex = MainWindow.TabControl.SelectedIndex;
			Cache.WindowWidth = (int)MainWindow.Width;
			Cache.WindowHeight = (int)MainWindow.Height;
			Cache.WindowIsFullScreen = MainWindow.WindowState == WindowState.Maximized;
			UserHandler.Instance.SaveCache();
		}
	}
}