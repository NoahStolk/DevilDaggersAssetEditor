using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Code;
using IrrKlang;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class AudioPreviewerControl : UserControl
	{
		private readonly ISoundEngine engine = new ISoundEngine();

		public ISound Song { get; private set; }
		public ISoundSource SongData { get; private set; }

		public bool Dragging { get; private set; }

		public AudioPreviewerControl()
		{
			InitializeComponent();

			Autoplay.IsChecked = UserHandler.Instance.cache.AudioPlayerIsAutoplayEnabled;

			ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
			ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;
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
			if (!IsInitialized)
				return;

			float pitch = (float)e.NewValue;

			PitchText.Text = $"x {pitch:0.00}";

			if (Song != null)
				Song.PlaybackSpeed = pitch;
		}

		private void ResetPitch_Click(object sender, RoutedEventArgs e)
		{
			PitchText.Text = "x 1.00";
			Pitch.Value = 1;

			if (Song != null)
				Song.PlaybackSpeed = 1;
		}

		private void Seek_DragStarted(object sender, DragStartedEventArgs e)
		{
			Dragging = true;
		}

		private void Seek_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Dragging = false;

			if (Song != null)
				Song.PlayPosition = (uint)Seek.Value;
		}

		public void Initialize(AudioAsset asset)
		{
			AudioName.Text = asset.AssetName;
			DefaultLoudness.Text = asset.PresentInDefaultLoudness ? asset.DefaultLoudness.ToString() : "N/A (Defaults to 1)";

			FileName.Text = File.Exists(asset.EditorPath) ? Path.GetFileName(asset.EditorPath) : Utils.FileNotFound;
			FileLoudness.Text = asset.Loudness.ToString();

			bool startPaused = !Autoplay.IsChecked ?? true;

			SongSet(asset.EditorPath, (float)Pitch.Value, startPaused);

			if (Song == null)
				return;

			ToggleImage.Source = ((Image)Resources[startPaused ? "PlayImage" : "PauseImage"]).Source;

			Seek.Maximum = Song.PlayLength;
			Seek.Value = 0;

			SeekText.Text = $"{EditorUtils.ToTimeString((int)Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Song.PlayLength)}";
			PitchText.Text = $"x {Song.PlaybackSpeed:0.00}";
		}

		private void SongSet(string filePath, float pitch, bool startPaused)
		{
			if (Song != null)
				Song.Stop();

			SongData = engine.GetSoundSource(filePath);
			Song = engine.Play2D(SongData, true, startPaused, true);

			if (Song != null)
			{
				Song.PlaybackSpeed = pitch;
				Song.PlayPosition = 0;
			}
		}

		private void Autoplay_ChangeState(object sender, RoutedEventArgs e) => UserHandler.Instance.cache.AudioPlayerIsAutoplayEnabled = Autoplay.IsChecked ?? false;
	}
}