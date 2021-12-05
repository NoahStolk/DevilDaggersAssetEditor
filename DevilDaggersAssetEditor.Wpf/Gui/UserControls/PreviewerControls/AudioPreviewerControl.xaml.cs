using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Audio;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Windows;
using OpenAlBindings;
using OpenAlBindings.Enums;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;

public partial class AudioPreviewerControl : UserControl, IPreviewerControl, IDisposable
{
	private readonly PlaybackDevice? _openAlDevice;

	private SoundObject? _soundObject;
	private bool _disposedValue;

	public AudioPreviewerControl()
	{
		const string openAlDll = "OpenAL32.dll";
		nint openAlHandle = LoadLibrary(openAlDll);
		if (openAlHandle != 0)
		{
			if (OpenAlDeviceHelper.PlaybackDevices.Length > 0)
			{
				_openAlDevice = OpenAlDeviceHelper.PlaybackDevices[0];
				_openAlDevice.MakeCurrent();

				Al.alListenerfv(FloatSourceProperty.AL_ORIENTATION, new float[] { 0, 0, 1, 0, 1, 0 });
			}
			else
			{
				App.LogError("No audio devices found.", null);
			}
		}
		else
		{
			App.LogError($"{openAlDll} was not found.", null);
		}

		InitializeComponent();

		Autoplay.IsChecked = UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled;

		ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
		ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;

		DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };
		timer.Tick += (sender, e) =>
		{
			if (_soundObject == null || _soundObject.State == SourceState.Paused)
				return;

			if (!IsDragging)
			{
				double length = GetSoundLength();
				Seek.Value = length == 0 ? 0 : GetSoundPosition() / length * Seek.Maximum;
			}

			SetSeekText();
		};
		timer.Start();
	}

	~AudioPreviewerControl()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: false);
	}

	public bool IsDragging { get; private set; }

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
	[DllImport("kernel32", SetLastError = true)]
	private static extern nint LoadLibrary(string lpFileName);
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments

	private void Toggle_Click(object sender, RoutedEventArgs e)
	{
		if (_soundObject == null)
			return;

		_soundObject.Toggle();
		ToggleImage.Source = ((Image)Resources[_soundObject.State == SourceState.Paused ? "PlayImage" : "PauseImage"]).Source;
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

		if (_soundObject != null)
			_soundObject.Offset = (float)Seek.Value;
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

	private double GetSoundPosition()
	{
		if (_soundObject == null)
			return 0;

		return _soundObject.Offset;
	}

	private double GetSoundLength()
	{
		if (_soundObject == null)
			return 0;

		Sound sound = _soundObject.Sound;
		int sampleCount = sound.Size / (sound.BitsPerSample / 8) / sound.Channels;
		return sampleCount / (double)sound.SampleRate;
	}

	private void SetSeekText() => SeekText.Content = $"{ToTimeString(GetSoundPosition())} / {ToTimeString(GetSoundLength())}";

	private void SongSet(string filePath, float pitch, bool startPaused)
	{
		_soundObject?.Delete();

		if (_openAlDevice == null || !File.Exists(filePath))
			return;

		Sound? sound = null;
		try
		{
			sound = new(filePath);
		}
		catch (WaveException ex)
		{
			Dispatcher.Invoke(() =>
			{
				ErrorWindow errorWindow = new("Cannot preview audio file.", "The .wav file could not be parsed.", ex);
				errorWindow.ShowDialog();
			});

			return;
		}

		_soundObject = new(sound);
		_soundObject.Looping = true;
		_soundObject.Pitch = pitch;

		if (!startPaused)
			_soundObject.Play();
	}

	private void Autoplay_ChangeState(object sender, RoutedEventArgs e)
		=> UserHandler.Instance.Cache.AudioPlayerIsAutoplayEnabled = Autoplay.IsChecked();

	private static string ToTimeString(double seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.Days > 0)
			return $"{timeSpan:dd\\:hh\\:mm\\:ss\\.fff}";
		if (timeSpan.Hours > 0)
			return $"{timeSpan:hh\\:mm\\:ss\\.fff}";
		return $"{timeSpan:mm\\:ss\\.fff}";
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_openAlDevice?.Delete();
			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
