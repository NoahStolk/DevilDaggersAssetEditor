using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersCore.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class MainWindow : Window
	{
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

			if (UserHandler.Instance.Cache.WindowWidth > 128)
				Width = UserHandler.Instance.Cache.WindowWidth;
			if (UserHandler.Instance.Cache.WindowHeight > 128)
				Height = UserHandler.Instance.Cache.WindowHeight;
			if (UserHandler.Instance.Cache.WindowIsFullScreen)
				WindowState = WindowState.Maximized;

			App.Instance.MainWindow = this;
			App.Instance.UpdateMainWindowTitle();
		}

		public AssetTabControl AudioAudioAssetTabControl { get; private set; } = null!;
		public AssetTabControl CoreShadersAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdModelBindingsAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdModelsAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdShadersAssetTabControl { get; private set; } = null!;
		public AssetTabControl DdTexturesAssetTabControl { get; private set; } = null!;
		public AssetTabControl ParticleParticlesAssetTabControl { get; private set; } = null!;

		public List<AssetTabControl> AssetTabControls { get; private set; } = null!;

		public Point CurrentTabControlSize { get; private set; }

		public double PathSize { get; private set; }
		public double DescriptionSize { get; private set; }
		public double TagsSize { get; private set; }

		public bool HasAnyAudioFiles()
			=> AudioAudioAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound);

		public bool HasAnyCoreFiles()
			=> CoreShadersAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound || (rc.Asset as ShaderAsset)!.EditorPathFragmentShader != GuiUtils.FileNotFound);

		public bool HasAnyDdFiles()
			=> DdModelBindingsAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound)
			|| DdModelsAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound)
			|| DdShadersAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound || (rc.Asset as ShaderAsset)!.EditorPathFragmentShader != GuiUtils.FileNotFound)
			|| DdTexturesAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound);

		public bool HasAnyParticleFiles()
			=> ParticleParticlesAssetTabControl.RowControls.Any(rc => rc.Asset.EditorPath != GuiUtils.FileNotFound);

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			AudioAudioAssetTabControl = new(BinaryFileType.Audio, AssetType.Audio, "Audio files (*.wav)|*.wav", "Audio");
			CoreShadersAssetTabControl = new(BinaryFileType.Core, AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");
			DdModelBindingsAssetTabControl = new(BinaryFileType.Dd, AssetType.ModelBinding, "Model binding files (*.txt)|*.txt", "Model Bindings");
			DdModelsAssetTabControl = new(BinaryFileType.Dd, AssetType.Model, "Model files (*.obj)|*.obj", "Models");
			DdShadersAssetTabControl = new(BinaryFileType.Dd, AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");
			DdTexturesAssetTabControl = new(BinaryFileType.Dd, AssetType.Texture, "Texture files (*.png)|*.png", "Textures");
			ParticleParticlesAssetTabControl = new(BinaryFileType.Particle, AssetType.Particle, "Particle files (*.bin)|*.bin", "Particles");

			AssetTabControls = new() { AudioAudioAssetTabControl, CoreShadersAssetTabControl, DdModelBindingsAssetTabControl, DdModelsAssetTabControl, DdShadersAssetTabControl, DdTexturesAssetTabControl, ParticleParticlesAssetTabControl };

			UpdateTextBoxSizes();

			TabControl.Items.Add(new TabItem { Header = "audio/Audio", Content = AudioAudioAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "core/Shaders", Content = CoreShadersAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Model Bindings", Content = DdModelBindingsAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Models", Content = DdModelsAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Shaders", Content = DdShadersAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Textures", Content = DdTexturesAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "particle/Particles", Content = ParticleParticlesAssetTabControl });

			TabControl.SelectedIndex = Math.Clamp(UserHandler.Instance.Cache.ActiveTabIndex, 0, AssetTabControls.Count - 1);

			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				UpdateRecommendedWindow updateRecommendedWindow = new(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
				updateRecommendedWindow.ShowDialog();
			}

			// After the window has loaded, some user controls still need to finish loading, so set a timer to make sure everything has loaded.
			// TODO: Find a better way to do this.
			DispatcherTimer timer = new() { Interval = new(0, 0, 0, 0, 10) };
			timer.Tick += (sender, e) =>
			{
				if (File.Exists(UserHandler.Instance.Cache.OpenedModFilePath))
				{
					List<UserAsset> assets = ModFileUtils.GetAssetsFromModFilePath(UserHandler.Instance.Cache.OpenedModFilePath);
					if (assets.Count > 0)
					{
						foreach (AssetTabControl tabHandler in AssetTabControls)
							tabHandler.UpdateAssetTabControls(assets);
					}
				}

				timer.Stop();
			};
			timer.Start();
		}

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

					_tabControlSizeIndex = i;
					return;
				}
			}
		}

		private void UpdateTextBoxSizes()
		{
			if (AssetTabControls == null)
				return;

			double columnWidth = (TabControl.Width - 32) / 20; // 20 columns
			double columnWidthAudio = columnWidth - 96 / 6f; // Loudness is 96 pixels in width, 8 is the grid star size of the column containing paths

			foreach (AssetTabControl tab in AssetTabControls)
			{
				double tagsWidth = columnWidth * 3;
				double descriptionWidth = columnWidth * 5;
				double pathWidth = (tab.AssetType == AssetType.Audio ? columnWidthAudio : columnWidth) * 6;

				foreach (AssetRowControl row in tab.RowControls)
				{
					row.GridTags.MaxWidth = tagsWidth;
					row.GridDescription.MaxWidth = descriptionWidth;
					row.GridPath.MaxWidth = pathWidth;
					row.GridPathFragment.MaxWidth = pathWidth;
				}
			}
		}
	}
}
