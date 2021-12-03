using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Extensions;
using IrrKlang;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;

public partial class AudioPreviewerControl : UserControl, IPreviewerControl, IDisposable
{
	private readonly ISoundEngine _engine = new();

	public AudioPreviewerControl()
	{
		InitializeComponent();

		Autoplay.IsChecked = UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled;

		ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
		ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;

		DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };
		timer.Tick += (sender, e) =>
		{
			if (Song?.Paused != false)
				return;

			if (!IsDragging)
			{
				float length = Song.PlayLength;
				if (length == 0)
					length = 1;
				Seek.Value = Song.PlayPosition / length * Seek.Maximum;
			}

			SeekText.Content = $"{EditorUtils.ToTimeString((int)Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Song.PlayLength)}";
		};
		timer.Start();
	}

	public ISound? Song { get; private set; }
	public ISoundSource? SongData { get; private set; }

	public bool IsDragging { get; private set; }

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
			_engine.Dispose();
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
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

		PitchText.Content = $"x {pitch:0.00}";

		if (Song != null)
			Song.PlaybackSpeed = pitch;
	}

	private void ResetPitch_Click(object sender, RoutedEventArgs e)
	{
		PitchText.Content = "x 1.00";
		Pitch.Value = 1;

		if (Song != null)
			Song.PlaybackSpeed = 1;
	}

	private void Seek_DragStarted(object sender, DragStartedEventArgs e)
	{
		IsDragging = true;
	}

	private void Seek_DragCompleted(object sender, DragCompletedEventArgs e)
	{
		IsDragging = false;

		if (Song != null)
			Song.PlayPosition = (uint)Seek.Value;
	}

	public void Initialize(AbstractAsset asset)
	{
		if (asset is not AudioAsset audioAsset)
			return;

		AudioName.Text = audioAsset.AssetName;
		DefaultLoudness.Content = audioAsset.PresentInDefaultLoudness ? audioAsset.DefaultLoudness.ToString() : "N/A (Defaults to 1)";

		FileName.Text = File.Exists(audioAsset.EditorPath) ? Path.GetFileName(audioAsset.EditorPath) : GuiUtils.FileNotFound;
		FileLoudness.Content = audioAsset.Loudness.ToString();

		bool startPaused = !Autoplay.IsChecked ?? true;

		SongSet(audioAsset.EditorPath, (float)Pitch.Value, startPaused);

		if (Song == null)
			return;

		ToggleImage.Source = ((Image)Resources[startPaused ? "PlayImage" : "PauseImage"]).Source;

		Seek.Maximum = Song.PlayLength;
		Seek.Value = 0;

		SeekText.Content = $"{EditorUtils.ToTimeString((int)Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Song.PlayLength)}";
		PitchText.Content = $"x {Song.PlaybackSpeed:0.00}";
	}

	private void SongSet(string filePath, float pitch, bool startPaused)
	{
		Song?.Stop();

		SongData = _engine.GetSoundSource(filePath);
		Song = _engine.Play2D(SongData, true, startPaused, true);

		if (Song != null)
		{
			Song.PlaybackSpeed = pitch;
			Song.PlayPosition = 0;
		}
	}

	private void Autoplay_ChangeState(object sender, RoutedEventArgs e)
		=> UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled = Autoplay.IsChecked();
}
