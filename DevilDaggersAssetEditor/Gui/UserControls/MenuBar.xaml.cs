using DevilDaggersAssetCore.BinaryFileAnalyzer;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Code.FileTabControlHandlers;
using DevilDaggersAssetEditor.Gui.Windows;
using DevilDaggersCore.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
#if DEBUG
using System.Windows.Media;
#endif

namespace DevilDaggersAssetEditor.Gui.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		private UserSettings settings => UserHandler.Instance.settings;

		public readonly List<AbstractFileTabControlHandler> tabHandlers;

		public MenuBarUserControl()
		{
			InitializeComponent();

			tabHandlers = App.Instance.Assembly
				.GetTypes()
				.Where(t => t.BaseType == typeof(AbstractFileTabControlHandler) && !t.IsAbstract)
				.OrderBy(t => t.Name)
				.Select(t => (AbstractFileTabControlHandler)Activator.CreateInstance(t))
				.ToList();

			foreach (AbstractFileTabControlHandler tabHandler in tabHandlers)
				FileMenuItem.Items.Add(tabHandler.CreateFileTypeMenuItem());

#if DEBUG
			MenuItem testException = new MenuItem { Header = "Test Exception", Background = new SolidColorBrush(Color.FromRgb(0, 255, 63)) };
			testException.Click += TestException_Click;

			MenuItem debug = new MenuItem { Header = "Debug", Background = new SolidColorBrush(Color.FromRgb(0, 255, 63)) };
			debug.Items.Add(testException);

			MenuPanel.Items.Add(debug);
#endif
		}

		private void AnalyzeBinaryFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			if (settings.EnableDevilDaggersRootFolder && Directory.Exists(settings.DevilDaggersRootFolder))
				openDialog.InitialDirectory = settings.DevilDaggersRootFolder;

			bool? openResult = openDialog.ShowDialog();
			if (openResult.HasValue && openResult.Value)
			{
				byte[] sourceFileBytes = File.ReadAllBytes(openDialog.FileName);

				AnalyzerFileResult result = BinaryFileAnalyzerWindow.TryReadResourceFile(openDialog.FileName, sourceFileBytes);
				if (result == null)
					result = BinaryFileAnalyzerWindow.TryReadParticleFile(openDialog.FileName, sourceFileBytes);

				if (result == null)
				{
					App.Instance.ShowMessage("File not recognized", "Make sure to open one of the following binary files: audio, core, dd, particle");
				}
				else
				{
					BinaryFileAnalyzerWindow fileAnalyzerWindow = new BinaryFileAnalyzerWindow(result);
					fileAnalyzerWindow.ShowDialog();
				}
			}
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new SettingsWindow();
			if (settingsWindow.ShowDialog() == true)
				UserHandler.Instance.SaveSettings();
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private void Changelog_Click(object sender, RoutedEventArgs e)
		{
			if (VersionHandler.Instance.VersionResult.Tool.Changelog != null)
			{
				ChangelogWindow changelogWindow = new ChangelogWindow();
				changelogWindow.ShowDialog();
			}
			else
			{
				App.Instance.ShowError("Changelog not retrieved", "The changelog has not been retrieved from DevilDaggers.info.");
			}
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			HelpWindow helpWindow = new HelpWindow();
			helpWindow.ShowDialog();
		}

		private void SourceCode_Click(object sender, RoutedEventArgs e) => Process.Start(UrlUtils.SourceCodeUrl(App.ApplicationName));

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new CheckingForUpdatesWindow();
			window.ShowDialog();

			VersionResult versionResult = VersionHandler.Instance.VersionResult;
			if (versionResult.IsUpToDate.HasValue)
			{
				if (!versionResult.IsUpToDate.Value)
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
					updateRecommendedWindow.ShowDialog();
				}
				else
				{
					App.Instance.ShowMessage("Up to date", $"{App.ApplicationDisplayName} {App.LocalVersion} is up to date.");
				}
			}
			else
			{
				App.Instance.ShowError($"Error retrieving version number for '{App.ApplicationName}'", versionResult.Exception.Message, versionResult.Exception.InnerException);
			}
		}

		private void ShowLog_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists("DDAE.log"))
				Process.Start("DDAE.log");
			else
				App.Instance.ShowMessage("No log file", "Log file does not exist.");
		}

		private void TestException_Click(object sender, RoutedEventArgs e) => throw new Exception("Test Exception");
	}
}