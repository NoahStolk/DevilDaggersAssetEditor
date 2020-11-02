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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class MainWindow : Window
	{
		private readonly List<Point> _tabControlSizes = new List<Point>
		{
			new Point(3840, 2160),
			new Point(2560, 1440),
			new Point(2048, 1152),
			new Point(1920, 1200),
			new Point(1920, 1080),
			new Point(1680, 1050),
			new Point(1440, 900),
			new Point(1366, 768),
		};

		public MainWindow()
		{
			InitializeComponent();

			TabControl.SelectedIndex = Math.Clamp(UserHandler.Instance.Cache.ActiveTabIndex, 0, 6);
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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			AudioAudioAssetTabControl = new AssetTabControl(BinaryFileType.Audio, AssetType.Audio, "Audio files (*.wav)|*.wav", "Audio");
			CoreShadersAssetTabControl = new AssetTabControl(BinaryFileType.Core, AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");
			DdModelBindingsAssetTabControl = new AssetTabControl(BinaryFileType.Dd, AssetType.ModelBinding, "Model binding files (*.txt)|*.txt", "Model Bindings");
			DdModelsAssetTabControl = new AssetTabControl(BinaryFileType.Dd, AssetType.Model, "Model files (*.obj)|*.obj", "Models");
			DdShadersAssetTabControl = new AssetTabControl(BinaryFileType.Dd, AssetType.Shader, "Shader files (*.glsl)|*.glsl", "Shaders");
			DdTexturesAssetTabControl = new AssetTabControl(BinaryFileType.Dd, AssetType.Texture, "Texture files (*.png)|*.png", "Textures");
			ParticleParticlesAssetTabControl = new AssetTabControl(BinaryFileType.Particle, AssetType.Particle, "Particle files (*.bin)|*.bin", "Particles");

			AssetTabControls = new List<AssetTabControl> { AudioAudioAssetTabControl, CoreShadersAssetTabControl, DdModelBindingsAssetTabControl, DdModelsAssetTabControl, DdShadersAssetTabControl, DdTexturesAssetTabControl, ParticleParticlesAssetTabControl };

			TabControl.Items.Add(new TabItem { Header = "audio/Audio", Content = AudioAudioAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "core/Shaders", Content = CoreShadersAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Model Bindings", Content = DdModelBindingsAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Models", Content = DdModelsAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Shaders", Content = DdShadersAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "dd/Textures", Content = DdTexturesAssetTabControl });
			TabControl.Items.Add(new TabItem { Header = "particle/Particles", Content = ParticleParticlesAssetTabControl });

			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
				updateRecommendedWindow.ShowDialog();
			}

			// After the window has loaded, some user controls still need to finish loading, so set a timer to make sure everything has loaded.
			// TODO: Find a better way to do this.
			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
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
					CurrentTabControlSize = size;
					TabControl.Width = size.X - 17;
					TabControl.Height = size.Y - 81;
					break;
				}
			}
		}
	}
}