using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersCore.Wpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class LoadingWindow : Window
	{
		private int _threadsComplete;
		private readonly List<BackgroundWorker> _threads = new();
		private readonly List<string> _threadMessages = new();

		public LoadingWindow()
		{
			InitializeComponent();

			VersionLabel.Text = $"Version {App.LocalVersion}";

#if DEBUG
			VersionLabel.Background = ColorUtils.ThemeColors["SuccessText"];
			VersionLabel.Text += " DEBUG";
#endif

			Loaded += RunThreads;
		}

		private void RunThreads(object? sender, EventArgs e)
		{
			using BackgroundWorker checkVersionThread = new();
			checkVersionThread.DoWork += (sender, e) => NetworkHandler.Instance.GetOnlineTool();
			checkVersionThread.RunWorkerCompleted += (sender, e) =>
			{
				Dispatcher.Invoke(() =>
				{
					string message = string.Empty;
					SolidColorBrush color;

					if (NetworkHandler.Instance.Tool == null)
					{
						message = "Error";
						color = ColorUtils.ThemeColors["ErrorText"];
					}
					else
					{
						if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumberRequired))
						{
							message = "Warning (update required)";
							color = ColorUtils.ThemeColors["WarningText"];
						}
						else if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
						{
							message = "Warning (update recommended)";
							color = ColorUtils.ThemeColors["SuggestionText"];
						}
						else
						{
							message = "OK (up to date)";
							color = ColorUtils.ThemeColors["SuccessText"];
						}
					}

					TaskResultsStackPanel.Children.Add(new TextBlock
					{
						Text = message,
						Foreground = color,
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			bool readUserSettingsSuccess = false;
			using BackgroundWorker readUserSettingsThread = new();
			readUserSettingsThread.DoWork += (sender, e) =>
			{
				try
				{
					UserHandler.Instance.ReadSettings();
					readUserSettingsSuccess = true;
				}
				catch (Exception ex)
				{
					App.Instance.ShowError("Error", "Error while trying to read user settings.", ex);
				}
			};
			readUserSettingsThread.RunWorkerCompleted += (sender, e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new TextBlock
					{
						Text = readUserSettingsSuccess ? File.Exists(UserSettings.FilePath) ? "OK (found user settings)" : "OK (created new user settings)" : "Error",
						Foreground = readUserSettingsSuccess ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["ErrorText"],
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			bool readUserCacheSuccess = false;
			using BackgroundWorker readUserCacheThread = new();
			readUserCacheThread.DoWork += (sender, e) =>
			{
				try
				{
					UserHandler.Instance.ReadCache();
					readUserCacheSuccess = true;
				}
				catch (Exception ex)
				{
					App.Instance.ShowError("Error", "Error while trying to read user cache.", ex);
				}
			};
			readUserCacheThread.RunWorkerCompleted += (sender, e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new TextBlock
					{
						Text = readUserCacheSuccess ? File.Exists(UserCache.FilePath) ? "OK (found user cache)" : "OK (created new user cache)" : "Error",
						Foreground = readUserCacheSuccess ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["ErrorText"],
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			bool retrieveModsSuccess = false;
			using BackgroundWorker retrieveModsThread = new();
			retrieveModsThread.DoWork += (sender, e) =>
			{
				Task<bool> modsTask = NetworkHandler.Instance.RetrieveModList();
				modsTask.Wait();
				retrieveModsSuccess = modsTask.Result;
			};
			retrieveModsThread.RunWorkerCompleted += (sender, e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new TextBlock
					{
						Text = retrieveModsSuccess ? "OK" : "Error",
						Foreground = retrieveModsSuccess ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["ErrorText"],
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			using BackgroundWorker mainInitThread = new();
			mainInitThread.DoWork += (sender, e) =>
			{
				Dispatcher.Invoke(() =>
				{
					MainWindow mainWindow = new();
					mainWindow.Show();
				});
			};
			mainInitThread.RunWorkerCompleted += (sender, e) => Close();

			_threads.Add(checkVersionThread);
			_threads.Add(readUserSettingsThread);
			_threads.Add(readUserCacheThread);
			_threads.Add(retrieveModsThread);
			_threads.Add(mainInitThread);

			_threadMessages.Add("Checking for updates...");
			_threadMessages.Add("Reading user settings...");
			_threadMessages.Add("Reading user cache...");
			_threadMessages.Add("Retrieving mods...");
			_threadMessages.Add("Initializing application...");

			RunThread(_threads[0]);
		}

		private void ThreadComplete()
		{
			_threadsComplete++;

			RunThread(_threads[_threadsComplete]);
		}

		private void RunThread(BackgroundWorker worker)
		{
			TasksStackPanel.Children.Add(new Label
			{
				Content = _threadMessages[_threadsComplete],
			});

			worker.RunWorkerAsync();
		}
	}
}
