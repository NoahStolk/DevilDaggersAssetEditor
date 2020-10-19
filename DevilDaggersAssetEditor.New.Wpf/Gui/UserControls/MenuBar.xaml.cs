using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Windows;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.New.Wpf.Gui.UserControls
{
	public partial class MenuBar : UserControl
	{
		public MenuBar()
		{
			InitializeComponent();

#if false
			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				HelpItem.Header += " (Update available)";
				HelpItem.FontWeight = FontWeights.Bold;

				foreach (MenuItem? menuItem in HelpItem.Items)
				{
					if (menuItem == null)
						continue;
					menuItem.FontWeight = FontWeights.Normal;
				}

				UpdateItem.Header = "Update available";
				UpdateItem.FontWeight = FontWeights.Bold;
			}

			TabHandlers = App.Assembly
				.GetTypes()
				.Where(t => t.BaseType == typeof(AbstractFileTabControlHandler) && !t.IsAbstract)
				.OrderBy(t => t.Name)
				.Select(t => (AbstractFileTabControlHandler)Activator.CreateInstance(t))
				.ToList();

			foreach (AbstractFileTabControlHandler tabHandler in TabHandlers)
				FileMenuItem.Items.Add(tabHandler.CreateFileTypeMenuItem());
#endif

#if DEBUG
			MenuItem debugItem = new MenuItem { Header = "Open debug window" };
			debugItem.Click += (sender, e) =>
			{
				DebugWindow debugWindow = new DebugWindow();
				debugWindow.ShowDialog();
			};

			MenuItem debugHeader = new MenuItem { Header = "Debug" };
			debugHeader.Items.Add(debugItem);

			MenuPanel.Items.Add(debugHeader);
#endif
		}

		private void AnalyzeBinaryFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
#if false
			OpenFileDialog openDialog = new OpenFileDialog();
			if (UserHandler.Instance.Settings.EnableDevilDaggersRootFolder && Directory.Exists(UserHandler.Instance.Settings.DevilDaggersRootFolder))
				openDialog.InitialDirectory = UserHandler.Instance.Settings.DevilDaggersRootFolder;

			bool? openResult = openDialog.ShowDialog();
			if (openResult.HasValue && openResult.Value)
			{
				byte[] sourceFileBytes = File.ReadAllBytes(openDialog.FileName);

				AnalyzerFileResult? result = BinaryFileAnalyzerWindow.TryReadResourceFile(openDialog.FileName, sourceFileBytes);
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
#endif
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
#if false
			SettingsWindow settingsWindow = new SettingsWindow();
			if (settingsWindow.ShowDialog() == true)
				UserHandler.Instance.SaveSettings();
#endif
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
#if false
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
#endif
		}

		private void Changelog_Click(object sender, RoutedEventArgs e)
		{
#if false
			if (NetworkHandler.Instance.Tool != null)
			{
				List<ChangelogEntry> changes = NetworkHandler.Instance.Tool.Changelog.Select(c => new ChangelogEntry(Version.Parse(c.VersionNumber), c.Date, MapToSharedModel(c.Changes).ToList())).ToList();
				ChangelogWindow changelogWindow = new ChangelogWindow(changes, App.LocalVersion);
				changelogWindow.ShowDialog();
			}
			else
			{
				App.Instance.ShowError("Changelog not retrieved", "The changelog has not been retrieved from DevilDaggers.info.");
			}

			static IEnumerable<Change>? MapToSharedModel(List<Clients.Change>? changes)
			{
				foreach (Clients.Change change in changes ?? new List<Clients.Change>())
					yield return new Change(change.Description, MapToSharedModel(change.SubChanges)?.ToList() ?? null);
			}
#endif
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
#if false
			HelpWindow helpWindow = new HelpWindow();
			helpWindow.ShowDialog();
#endif
		}

		private void SourceCode_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SourceCodeUrl(App.ApplicationName).ToString());

		private void Update_Click(object sender, RoutedEventArgs e)
		{
#if false
			CheckingForUpdatesWindow window = new CheckingForUpdatesWindow(NetworkHandler.Instance.GetOnlineTool);
			window.ShowDialog();

			if (NetworkHandler.Instance.Tool != null)
			{
				if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
					updateRecommendedWindow.ShowDialog();
				}
				else
				{
					App.Instance.ShowMessage("Up to date", $"{App.ApplicationDisplayName} {App.LocalVersion} is up to date.");
				}
			}
			else
			{
				App.Instance.ShowError("Error retrieving tool information", "An error occurred while attempting to retrieve tool information from the API.");
			}
#endif
		}
	}
}