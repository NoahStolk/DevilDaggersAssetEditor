using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.Web;
using DevilDaggersAssetEditor.GUI.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using DevilDaggersAssetEditor.Code.TabControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		private readonly List<AbstractTabControlHandler> tabHandlers;

		public MenuBarUserControl()
		{
			InitializeComponent();

			tabHandlers = new List<AbstractTabControlHandler>();
			foreach (Type type in App.Instance.Assembly.GetTypes().Where(t => t.BaseType == typeof(AbstractTabControlHandler) && !t.IsAbstract))
				tabHandlers.Add((AbstractTabControlHandler)Activator.CreateInstance(type));

			foreach (AbstractTabControlHandler tabHandler in tabHandlers)
				FileMenuItem.Items.Add(tabHandler.CreateFileTypeMenuItem());

#if DEBUG
			MenuItem testException = new MenuItem { Header = "Test Exception", Background = new SolidColorBrush(Color.FromRgb(0, 255, 64)) };
			testException.Click += TestException_Click;

			MenuItem debug = new MenuItem { Header = "Debug", Background = new SolidColorBrush(Color.FromRgb(0, 255, 64)) };
			debug.Items.Add(testException);

			MenuPanel.Items.Add(debug);
#endif
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private void SourceCode_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.SourceCode);
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new CheckingForUpdatesWindow();
			window.ShowDialog();

			if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue)
			{
				if (!NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
					updateRecommendedWindow.ShowDialog();
				}
				else
				{
					App.Instance.ShowMessage("Up to date", $"{ApplicationUtils.ApplicationDisplayNameWithVersion} is up to date.");
				}
			}
		}

		private void ShowLog_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists("DDAE.log"))
				Process.Start("DDAE.log");
			else
				App.Instance.ShowMessage("No log file", "Log file does not exist.");
		}

		private void TestException_Click(object sender, RoutedEventArgs e)
		{
			throw new Exception("Test Exception");
		}
	}
}