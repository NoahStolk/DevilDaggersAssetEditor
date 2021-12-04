using OpenAlBindings;
using OpenAlBindings.Enums;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public class SoundObject
{
	private float _pitch = 1;
	private float _gain = 1;

	private uint _sourceId;

	public SoundObject(Sound sound)
	{
		Sound = sound;
		_sourceId = Sound.CreateSource();

		SetPitch();
		SetGain();
	}

	public Sound Sound { get; }

	public float Pitch
	{
		get => _pitch;
		set
		{
			_pitch = value;
			SetPitch();
		}
	}

	public float Gain
	{
		get => _gain;
		set
		{
			_gain = value;
			SetGain();
		}
	}

	public SourceState State
	{
		get
		{
			if (_sourceId == 0)
				return SourceState.Uninitialized;

			Al.alGetSourcei(_sourceId, IntSourceProperty.AL_SOURCE_STATE, out int state);
			return (SourceState)state;
		}
	}

	public bool Looping
	{
		get
		{
			Al.alGetSourcei(_sourceId, IntSourceProperty.AL_LOOPING, out int value);
			return value == 1;
		}
		set => Al.alSourcei(_sourceId, IntSourceProperty.AL_LOOPING, value ? 1 : 0);
	}

	private void SetPitch()
		=> Al.alSourcef(_sourceId, FloatSourceProperty.AL_PITCH, Pitch);

	private void SetGain()
		=> Al.alSourcef(_sourceId, FloatSourceProperty.AL_GAIN, Gain);

	public void Play()
	{
		if (State != SourceState.Playing)
			Al.alSourcePlay(_sourceId);
	}

	public void Stop()
	{
		if (State != SourceState.Stopped)
			Al.alSourceStop(_sourceId);
	}

	public void Toggle()
	{
		if (State == SourceState.Paused)
			Al.alSourcePlay(_sourceId);
		else if (State == SourceState.Playing)
			Al.alSourcePause(_sourceId);
	}

	public void Delete()
	{
		if (_sourceId == 0)
			return;

		Stop();

		uint[] buffers = new uint[1];
		Al.alSourceUnqueueBuffers(_sourceId, 1, buffers);
		Al.alDeleteBuffers(1, buffers);

		uint[] sources = new uint[1] { _sourceId };
		Al.alDeleteSources(1, sources);
		_sourceId = 0;
	}
}
