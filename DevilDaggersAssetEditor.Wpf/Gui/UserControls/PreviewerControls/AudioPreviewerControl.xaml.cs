using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Audio;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;

public partial class AudioPreviewerControl : UserControl, IPreviewerControl
{
	private SoundObject? _soundObject;

	public AudioPreviewerControl()
	{
		AudioEngine.Initialize();

		InitializeComponent();

		Autoplay.IsChecked = UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled;

		ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
		ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;

		DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };
		timer.Tick += (sender, e) =>
		{
			if (_soundObject == null || _soundObject.State == OpenAlBindings.Enums.SourceState.Paused)
				return;

			if (!IsDragging)
			{
				float length = GetSoundLength();
				if (length == 0)
					length = 1;
				Seek.Value = GetSoundPosition() / length * Seek.Maximum;
			}

			SetSeekText();
		};
		timer.Start();
	}

	public bool IsDragging { get; private set; }

	private void Toggle_Click(object sender, RoutedEventArgs e)
	{
		if (_soundObject == null)
			return;

		_soundObject.Toggle();
		ToggleImage.Source = ((Image)Resources[_soundObject.State == OpenAlBindings.Enums.SourceState.Paused ? "PlayImage" : "PauseImage"]).Source;
	}

	private void Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!IsInitialized)
			return;

		float pitch = (float)e.NewValue;

		PitchText.Content = $"x {pitch:0.00}";

		if (_soundObject != null)
			_soundObject.Pitch = pitch;
	}

	private void ResetPitch_Click(object sender, RoutedEventArgs e)
	{
		PitchText.Content = "x 1.00";
		Pitch.Value = 1;

		if (_soundObject != null)
			_soundObject.Pitch = 1;
	}

	private void Seek_DragStarted(object sender, DragStartedEventArgs e)
	{
		IsDragging = true;
	}

	private void Seek_DragCompleted(object sender, DragCompletedEventArgs e)
	{
		IsDragging = false;

		//if (Song != null)
		//	Song.PlayPosition = (uint)Seek.Value;
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

		if (_soundObject == null)
			return;

		ToggleImage.Source = ((Image)Resources[startPaused ? "PlayImage" : "PauseImage"]).Source;

		Seek.Maximum = GetSoundLength();
		Seek.Value = 0;

		SetSeekText();
		PitchText.Content = $"x {_soundObject.Pitch:0.00}";
	}

	// TODO
	private int GetSoundPosition() => 0;

	// TODO
	private int GetSoundLength() => _soundObject?.Sound.Size ?? 0;

	private void SetSeekText() => SeekText.Content = $"{EditorUtils.ToTimeString(GetSoundPosition())} / {EditorUtils.ToTimeString(GetSoundLength())}";

	private void SongSet(string filePath, float pitch, bool startPaused)
	{
		_soundObject?.Delete();

		if (!File.Exists(filePath))
			return;

		_soundObject = new(new Sound(filePath));
		_soundObject.Looping = true;
		_soundObject.Pitch = pitch;

		if (!startPaused)
			_soundObject.Play();
	}

	private void Autoplay_ChangeState(object sender, RoutedEventArgs e)
		=> UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled = Autoplay.IsChecked();
}
