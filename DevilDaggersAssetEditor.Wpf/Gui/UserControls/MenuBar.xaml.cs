using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Models;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		private const string _modFileFilter = "Devil Daggers Asset Editor mod files (*.ddae)|*.ddae";

		public MenuBarUserControl()
		{
			InitializeComponent();

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

		private void ExtractBinaries_Click(object sender, RoutedEventArgs e)
		{
			ExtractBinariesWindow window = new ExtractBinariesWindow();
			window.ShowDialog();
		}

		private void MakeBinaries_Click(object sender, RoutedEventArgs e)
		{
			MakeBinariesWindow window = new MakeBinariesWindow();
			window.ShowDialog();
		}

		private void ImportAssets_Click(object sender, RoutedEventArgs e)
		{
			ImportAssetsWindow window = new ImportAssetsWindow();
			window.ShowDialog();
		}

		private void OpenMod_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog { Filter = _modFileFilter };
			dialog.OpenModsRootFolder();

			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			List<UserAsset> userAssets = ModFileUtils.GetAssetsFromModFilePath(dialog.FileName);
			if (userAssets.Count == 0)
				return;

			foreach (AssetTabControl assetTabControl in App.Instance.MainWindow!.AssetTabControls)
			{
				foreach (AssetRowControl rowHandler in assetTabControl.RowControls)
				{
					AbstractAsset asset = rowHandler.Asset;
					UserAsset? userAsset = userAssets.Find(a => a.AssetName == asset.AssetName && a.AssetType == asset.AssetType);
					if (userAsset != null)
					{
						asset.ImportValuesFromUserAsset(userAsset);

						rowHandler.UpdateGui();
					}
				}
			}
		}

		private void SaveMod_Click(object sender, RoutedEventArgs e)
		{
			List<AbstractAsset> assets = App.Instance.MainWindow!.AssetTabControls.SelectMany(atc => atc.GetAssets()).ToList();

			List<UserAsset> userAssets = new List<UserAsset>();
			foreach (AbstractAsset asset in assets)
				userAssets.Add(asset.ToUserAsset());

			SaveFileDialog dialog = new SaveFileDialog { Filter = _modFileFilter };
			dialog.OpenModsRootFolder();

			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			JsonFileUtils.SerializeToFile(dialog.FileName, userAssets, true);
		}

		private void ImportAudioLoudness_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ExportAudioLoudness_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Exit_Click(object sender, RoutedEventArgs e)
			=> Application.Current.Shutdown();

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new SettingsWindow();
			if (settingsWindow.ShowDialog() == true)
				UserHandler.Instance.SaveSettings();
		}

		private void AnalyzeBinaryFile_Click(object sender, RoutedEventArgs e)
		{
			BinaryFileAnalyzerWindow fileAnalyzerWindow = new BinaryFileAnalyzerWindow();
			fileAnalyzerWindow.ShowDialog();
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			HelpWindow helpWindow = new HelpWindow();
			helpWindow.ShowDialog();
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private void Changelog_Click(object sender, RoutedEventArgs e)
		{
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
		}

		private void ViewSourceCode_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SourceCodeUrl(App.ApplicationName).ToString());

		private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
		{
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
		}
	}
}