using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Extensions;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;

public partial class AudioPreviewerControl : UserControl, IPreviewerControl, IDisposable
{
	private readonly WaveOutEvent _outputDevice = new();
	private SmbPitchShiftingSampleProvider? _pitch;
	private AudioFileReader? _audioFile;

	public AudioPreviewerControl()
	{
		InitializeComponent();

		Autoplay.IsChecked = UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled;

		ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
		ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;

		DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };
		timer.Tick += (sender, e) =>
		{
			if (_outputDevice.PlaybackState != PlaybackState.Playing || _audioFile == null)
				return;

			if (!IsDragging)
			{
				float length = _audioFile.Length;
				if (length == 0)
					length = 1;
				Seek.Value = _audioFile.Position / length * Seek.Maximum;
			}

			SeekText.Content = $"{EditorUtils.ToTimeString((int)_audioFile.Position)} / {EditorUtils.ToTimeString((int)_audioFile.Length)}";
		};
		timer.Start();
	}

	public bool IsDragging { get; private set; }

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_outputDevice.Dispose();
			_audioFile?.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Toggle_Click(object sender, RoutedEventArgs e)
	{
		bool wasPlaying = _outputDevice.PlaybackState == PlaybackState.Playing;
		if (wasPlaying)
			_outputDevice.Pause();
		else
			_outputDevice.Play();

		ToggleImage.Source = ((Image)Resources[wasPlaying ? "PlayImage" : "PauseImage"]).Source;
	}

	private void Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!IsInitialized)
			return;

		float pitch = (float)e.NewValue;

		PitchText.Content = $"x {pitch:0.00}";

		if (_pitch != null)
			_pitch.PitchFactor = pitch;
	}

	private void ResetPitch_Click(object sender, RoutedEventArgs e)
	{
		PitchText.Content = "x 1.00";
		Pitch.Value = 1;

		if (_pitch != null)
			_pitch.PitchFactor = 1;
	}

	private void Seek_DragStarted(object sender, DragStartedEventArgs e)
	{
		IsDragging = true;
	}

	private void Seek_DragCompleted(object sender, DragCompletedEventArgs e)
	{
		IsDragging = false;

		if (_audioFile != null)
			_audioFile.Position = (long)Seek.Value;
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

		if (_audioFile == null)
			return;

		ToggleImage.Source = ((Image)Resources[startPaused ? "PlayImage" : "PauseImage"]).Source;

		Seek.Maximum = _audioFile.Length;
		Seek.Value = 0;

		SeekText.Content = $"{EditorUtils.ToTimeString((int)_audioFile.Position)} / {EditorUtils.ToTimeString((int)_audioFile.Length)}";
		PitchText.Content = $"x {_pitch.PitchFactor:0.00}";
	}

	private void SongSet(string filePath, float pitch, bool startPaused)
	{
		_outputDevice.Stop();

		if (!File.Exists(filePath))
			return;

		_audioFile = new AudioFileReader(filePath);
		_pitch = new SmbPitchShiftingSampleProvider(_audioFile.ToSampleProvider())
		{
			PitchFactor = pitch,
		};
		_outputDevice.Init(_pitch);

		if (!startPaused)
			_outputDevice.Play();
	}

	private void Autoplay_ChangeState(object sender, RoutedEventArgs e)
		=> UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled = Autoplay.IsChecked();
}
