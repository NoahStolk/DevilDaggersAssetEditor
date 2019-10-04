using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;
using DevilDaggersAssetCore.Assets;
using System.Windows.Controls.Primitives;
using IrrKlang;
using System.Windows.Threading;
using System.IO;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetTabControls
{
	public partial class AudioAssetTabControl : UserControl
	{
		private readonly ISoundEngine engine = new ISoundEngine();

		public ISound Song { get; private set; }
		public ISoundSource SongData { get; private set; }

		private bool dragging;

		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(AudioAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AudioAssetTabControlHandler Handler { get; private set; }

		public AudioAssetTabControl()
		{
			InitializeComponent();
			ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
			ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;

			DispatcherTimer timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 10)
			};
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (Song != null && !Song.Paused)
			{
				if (!dragging)
					Seek.Value = Song.PlayPosition / (float)Song.PlayLength * Seek.Maximum;

				SeekText.Text = $"{ToTimeString((int)Song.PlayPosition)} / {ToTimeString((int)Song.PlayLength)}";
			}
		}

		private string ToTimeString(int milliseconds)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, milliseconds);
			if (timeSpan.Days > 0)
				return $"{timeSpan:dd\\:hh\\:mm\\:ss\\.fff}";
			if (timeSpan.Hours > 0)
				return $"{timeSpan:hh\\:mm\\:ss\\.fff}";
			return $"{timeSpan:mm\\:ss\\.fff}";
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (AudioAssetControl ac in Handler.CreateUserControls())
			{
				AssetEditor.Children.Add(ac);
				ac.MouseDoubleClick += (senderAC, eAC) => Ac_MouseDoubleClick(ac.Handler.Asset);
			}
		}

		private void Ac_MouseDoubleClick(AudioAsset audioAsset)
		{
			AudioName.Text = audioAsset.AssetName;
			DefaultLoudness.Text = audioAsset.PresentInDefaultLoudness ? audioAsset.DefaultLoudness.ToString() : "N/A (1)";
			AudioFile.Text = audioAsset.EditorPath.IsPathValid() ? Path.GetFileName(audioAsset.EditorPath) : audioAsset.EditorPath;

			if (Song != null)
				Song.Stop();

			bool startPaused = !Autoplay.IsChecked ?? true;
			SongData = engine.GetSoundSource(audioAsset.EditorPath);
			Song = engine.Play2D(SongData, true, startPaused, true);

			if (Song != null)
			{
				ToggleImage.Source = ((Image)Resources[startPaused ? "PlayImage" : "PauseImage"]).Source;

				Seek.Maximum = Song.PlayLength;
				Song.PlaybackSpeed = (float)Pitch.Value;
				Song.PlayPosition = 0;
				Seek.Value = 0;

				SeekText.Text = $"{ToTimeString((int)Song.PlayPosition)} / {ToTimeString((int)Song.PlayLength)}";
				PitchText.Text = $"x {Song.PlaybackSpeed:0.00}";
			}
		}

		private void Toggle_Click(object sender, RoutedEventArgs e)
		{
			if (Song != null)
			{
				Song.Paused = !Song.Paused;
				ToggleImage.Source = ((Image)Resources[Song.Paused ? "PlayImage" : "PauseImage"]).Source;
			}
		}

		private void Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			float pitch = (float)e.NewValue;
			if (Song != null)
				Song.PlaybackSpeed = pitch;

			if (PitchText != null) // Doesn't exist yet when initializing control.
				PitchText.Text = $"x {pitch:0.00}";
		}

		private void ResetPitch_Click(object sender, RoutedEventArgs e)
		{
			if (Song != null)
				Song.PlaybackSpeed = 1;
			PitchText.Text = "x 1.00";
			Pitch.Value = 1;
		}

		private void Seek_DragStarted(object sender, DragStartedEventArgs e)
		{
			dragging = true;
		}

		private void Seek_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			dragging = false;

			if (Song != null)
				Song.PlayPosition = (uint)Seek.Value;
		}
	}
}