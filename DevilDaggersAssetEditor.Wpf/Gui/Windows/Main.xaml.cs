using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;
using DevilDaggersAssetEditor.Wpf.ModFiles;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Models;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class MainWindow : Window
	{
		public static readonly RoutedUICommand NewCommand = new("New", nameof(NewCommand), typeof(MainWindow), new() { new KeyGesture(Key.N, ModifierKeys.Control) });
		public static readonly RoutedUICommand OpenCommand = new("Open", nameof(OpenCommand), typeof(MainWindow), new() { new KeyGesture(Key.O, ModifierKeys.Control) });
		public static readonly RoutedUICommand SaveCommand = new("Save", nameof(SaveCommand), typeof(MainWindow), new() { new KeyGesture(Key.S, ModifierKeys.Control) });
		public static readonly RoutedUICommand SaveAsCommand = new("Save as", nameof(SaveAsCommand), typeof(MainWindow), new() { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
		public static readonly RoutedUICommand OpenManagerCommand = new("Open mod manager", nameof(OpenManagerCommand), typeof(MainWindow), new() { new KeyGesture(Key.I, ModifierKeys.Control) });
		public static readonly RoutedUICommand ExtractBinariesCommand = new("Extract binaries", nameof(ExtractBinariesCommand), typeof(MainWindow), new() { new KeyGesture(Key.E, ModifierKeys.Control) });
		public static readonly RoutedUICommand MakeBinariesCommand = new("Make binaries", nameof(MakeBinariesCommand), typeof(MainWindow), new() { new KeyGesture(Key.M, ModifierKeys.Control) });
		public static readonly RoutedUICommand ImportAssetsCommand = new("Import assets", nameof(ImportAssetsCommand), typeof(MainWindow), new() { new KeyGesture(Key.I, ModifierKeys.Control | ModifierKeys.Shift) });
		public static readonly RoutedUICommand ExitCommand = new("Exit", nameof(ExitCommand), typeof(MainWindow), new() { new KeyGesture(Key.F4, ModifierKeys.Alt) });

		private int _tabControlSizeIndex;

		private readonly List<Point> _tabControlSizes = new()
		{
			new(3840, 2160),
			new(2560, 1440),
			new(2048, 1152),
			new(1920, 1200),
			new(1920, 1080),
			new(1680, 1050),
			new(1440, 900),
			new(1366, 768),
		};

		public MainWindow()
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
			MenuItem debugItem = new() { Header = "Open debug window" };
			debugItem.Click += (sender, e) =>
			{
				DebugWindow debugWindow = new();
				debugWindow.ShowDialog();
			};

			MenuItem debugHeader = new() { Header = "Debug" };
			debugHeader.Items.Add(debugItem);

			MenuPanel.Items.Add(debugHeader);
#endif

			if (UserHandler.Instance.Cache.WindowWidth > 128)
				Width = UserHandler.Instance.Cache.WindowWidth;
			if (UserHandler.Instance.Cache.WindowHeight > 128)
				Height = UserHandler.Instance.Cache.WindowHeight;
			if (UserHandler.Instance.Cache.WindowIsFullScreen)
				WindowState = WindowState.Maximized;

			App.Instance.MainWindow = this;
			App.Instance.UpdateMainWindowTitle();
		}

		public TabControl TabControl { get; private set; } = new() { Width = 1366, Height = 768 };

		public AssetTabControl AudioAudioAssetTabControl { get; private set; } = null!;
		public AssetTabControl CoreShadersAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdModelBindingsAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdModelsAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdShadersAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdTexturesAssetTabControl { get; private set; } = null!;

		public List<AssetTabControl> AssetTabControls { get; private set; } = null!;

		public Point CurrentTabControlSize { get; private set; }

		public double PathSize { get; private set; }
		public double DescriptionSize { get; private set; }
		public double TagsSize { get; private set; }

		public bool HasLoaded { get; private set; }

		public bool HasAnyAudioFiles()
			=> AudioAudioAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound);

		public bool HasAnyCoreFiles()
			=> CoreShadersAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound || (rc.Asset as ShaderAsset)!.EditorPathFragmentShader != GuiUtils.FileNotFound);

		public bool HasAnyDdFiles()
			=> DdModelBindingsAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound)
			|| DdModelsAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound)
			|| DdShadersAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound || (rc.Asset as ShaderAsset)!.EditorPathFragmentShader != GuiUtils.FileNotFound)
			|| DdTexturesAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound);

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ModFileHandler.Instance.FileOpen(UserHandler.Instance.Cache.OpenedModFilePath);
			ModFileHandler.Instance.UpdateModFileState(UserHandler.Instance.Cache.OpenedModFilePath);

			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				UpdateRecommendedWindow updateRecommendedWindow = new(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
				updateRecommendedWindow.ShowDialog();
			}

			using BackgroundWorker heavyGuiThread = new();
			heavyGuiThread.DoWork += (sender, e) =>
			{
				Dispatcher.Invoke(() =>
				{
					AudioAudioAssetTabControl = new(BinaryType.Audio, AssetType.Audio, "Audio files (*.wav)|*.wav", "Audio");
					CoreShadersAssetTabControl = new(BinaryType.Core, AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");
					DdModelBindingsAssetTabControl = new(BinaryType.Dd, AssetType.ModelBinding, "Model binding files (*.txt)|*.txt", "Model Bindings");
					DdModelsAssetTabControl = new(BinaryType.Dd, AssetType.Model, "Model files (*.obj)|*.obj", "Models");
					DdShadersAssetTabControl = new(BinaryType.Dd, AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");
					DdTexturesAssetTabControl = new(BinaryType.Dd, AssetType.Texture, "Texture files (*.png)|*.png", "Textures");

					AssetTabControls = new() { AudioAudioAssetTabControl, CoreShadersAssetTabControl, DdModelBindingsAssetTabControl, DdModelsAssetTabControl, DdShadersAssetTabControl, DdTexturesAssetTabControl };

					ScrollViewerMain.Content = TabControl;
					TabControl.Items.Add(new TabItem { Header = "audio/Audio", Content = AudioAudioAssetTabControl });
					TabControl.Items.Add(new TabItem { Header = "core/Shaders", Content = CoreShadersAssetTabControl });
					TabControl.Items.Add(new TabItem { Header = "dd/Model Bindings", Content = DdModelBindingsAssetTabControl });
					TabControl.Items.Add(new TabItem { Header = "dd/Models", Content = DdModelsAssetTabControl });
					TabControl.Items.Add(new TabItem { Header = "dd/Shaders", Content = DdShadersAssetTabControl });
					TabControl.Items.Add(new TabItem { Header = "dd/Textures", Content = DdTexturesAssetTabControl });

					TabControl.SelectedIndex = Math.Clamp(UserHandler.Instance.Cache.ActiveTabIndex, 0, AssetTabControls.Count - 1);

					HasLoaded = true;
				});
			};
			heavyGuiThread.RunWorkerCompleted += (sender, e) =>
			{
				UpdateTextBoxSizes();
				UpdateHeights();
			};
			heavyGuiThread.RunWorkerAsync();
		}

		#region Events

		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
			=> e.CanExecute = true;

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ModFileHandler.Instance.ProceedWithUnsavedChanges())
				return;

			foreach (AssetTabControl assetTabControl in App.Instance.MainWindow!.AssetTabControls)
			{
				foreach (AssetRowControl assetRowControl in assetTabControl.RowControls)
				{
					AbstractAsset asset = assetRowControl.Asset;

					asset.EditorPath = GuiUtils.FileNotFound;
					if (asset is ShaderAsset shaderAsset)
						shaderAsset.EditorPathFragmentShader = GuiUtils.FileNotFound;

					assetRowControl.UpdateGui();
				}
			}

			ModFileHandler.Instance.ModFile.Clear();

			ModFileHandler.Instance.UpdateModFileState(string.Empty);
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ModFileHandler.Instance.ProceedWithUnsavedChanges())
				return;

			OpenFileDialog dialog = new() { Filter = GuiUtils.ModFileFilter };
			dialog.OpenModsRootFolder();

			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			ModFileHandler.Instance.FileOpen(dialog.FileName);
			if (ModFileHandler.Instance.ModFile.Count == 0)
				return;

			foreach (AssetTabControl assetTabControl in App.Instance.MainWindow!.AssetTabControls)
				assetTabControl.UpdateAssetTabControls(ModFileHandler.Instance.ModFile);
		}

		private void OpenManager_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ModManagerWindow window = new();
			window.ShowDialog();
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
			=> ModFileHandler.Instance.FileSave();

		private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
			=> ModFileHandler.Instance.FileSaveAs();

		private void ExtractBinaries_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ExtractBinariesWindow window = new();
			window.ShowDialog();
		}

		private void MakeBinaries_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MakeBinariesWindow window = new();
			window.ShowDialog();
		}

		private void ImportAssets_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ImportAssetsWindow window = new();
			window.ShowDialog();
		}

		private void ImportAudioLoudness_Click(object sender, RoutedEventArgs e)
			=> LoudnessWpfUtils.ImportLoudness(App.Instance.MainWindow!.AudioAudioAssetTabControl.RowControls);

		private void ExportAudioLoudness_Click(object sender, RoutedEventArgs e)
			=> LoudnessWpfUtils.ExportLoudness(App.Instance.MainWindow!.AudioAudioAssetTabControl.RowControls);

		private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
			=> Application.Current.Shutdown();

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new();
			if (settingsWindow.ShowDialog() == true)
				UserHandler.Instance.SaveSettings();
		}

		private void AnalyzeBinaryFile_Click(object sender, RoutedEventArgs e)
		{
			BinaryFileAnalyzerWindow window = new();
			window.ShowDialog();
		}

		private void TrimBinaryFile_Click(object sender, RoutedEventArgs e)
		{
			TrimBinaryWindow window = new();
			window.ShowDialog();
		}

		private void Help_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl("https://devildaggers.info/Wiki/Guides/AssetEditor");

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new();
			aboutWindow.ShowDialog();
		}

		private void Changelog_Click(object sender, RoutedEventArgs e)
		{
			if (NetworkHandler.Instance.Tool != null)
			{
				List<ChangelogEntry> changes = NetworkHandler.Instance.Tool.Changelog.ConvertAll(c => new ChangelogEntry(Version.Parse(c.VersionNumber), c.Date, MapToSharedModel(c.Changes)?.ToList() ?? new()));
				ChangelogWindow changelogWindow = new(changes, App.LocalVersion);
				changelogWindow.ShowDialog();
			}
			else
			{
				App.Instance.ShowError("Changelog not retrieved", "The changelog has not been retrieved from DevilDaggers.info.");
			}

			static IEnumerable<Change>? MapToSharedModel(List<Clients.Change>? changes)
			{
				foreach (Clients.Change change in changes ?? new())
					yield return new(change.Description, MapToSharedModel(change.SubChanges)?.ToList());
			}
		}

		private void ViewSourceCode_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SourceCodeUrl(App.ApplicationName).ToString());

		private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new(NetworkHandler.Instance.GetOnlineTool);
			window.ShowDialog();

			if (NetworkHandler.Instance.Tool != null)
			{
				if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
				{
					UpdateRecommendedWindow updateRecommendedWindow = new(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
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

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = ModFileHandler.Instance.ProceedWithUnsavedChanges();
		}

		#endregion Events

		#region GUI Responsiveness

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			for (int i = 0; i < _tabControlSizes.Count; i++)
			{
				Point size = _tabControlSizes[i];
				if (i == _tabControlSizes.Count - 1 || ActualWidth >= size.X && ActualHeight >= size.Y - 24)
				{
					if (_tabControlSizeIndex == i)
						return;

					CurrentTabControlSize = size;
					TabControl.Width = size.X - 17;
					TabControl.Height = size.Y - 81;

					UpdateTextBoxSizes();
					UpdateHeights();

					_tabControlSizeIndex = i;
					return;
				}
			}
		}

		private void UpdateTextBoxSizes()
		{
			if (AssetTabControls == null)
				return;

			double columnWidth = (TabControl.Width - 32) / 20; // Consists of 20 columns.
			double columnWidthAudio = columnWidth - 96 / 6f; // Loudness is 96 pixels in width. 8 is the grid star size of the column containing paths.

			foreach (AssetTabControl tab in AssetTabControls)
			{
				double tagsWidth = columnWidth * 3;
				double descriptionWidth = columnWidth * 5;
				double pathWidth = (tab.AssetType == AssetType.Audio ? columnWidthAudio : columnWidth) * 6;

				foreach (AssetRowControl row in tab.RowControls)
				{
					row.TextBlockTags.MaxWidth = tagsWidth;
					row.TextBlockDescription.MaxWidth = descriptionWidth;
					row.GridPath.MaxWidth = pathWidth;
					row.GridPathFragment.MaxWidth = pathWidth;
				}
			}
		}

		private void UpdateHeights()
		{
			if (AssetTabControls == null)
				return;

			double heightLeft = TabControl.Height - 192; // Tag filters area is 192 pixels in height.
			double previewerHeight = heightLeft * 0.3f;
			double assetsHeight = heightLeft * 0.7f;

			foreach (AssetTabControl tab in AssetTabControls)
			{
				tab.PreviewRowDefinition.MaxHeight = previewerHeight;
				tab.AssetsRowDefinition.MaxHeight = assetsHeight;

				if (tab.Previewer is TexturePreviewerControl texturePreviewer)
				{
					texturePreviewer.PreviewImage.MaxWidth = previewerHeight - 32;
					texturePreviewer.PreviewImage.MaxHeight = previewerHeight - 32;
				}
				else if (tab.Previewer is ShaderPreviewerControl shaderPreviewer)
				{
					shaderPreviewer.ScrollViewerVertex.MaxHeight = previewerHeight - 32;
					shaderPreviewer.ScrollViewerFragment.MaxHeight = previewerHeight - 32;
				}
			}
		}

		#endregion GUI Responsiveness
	}
}
