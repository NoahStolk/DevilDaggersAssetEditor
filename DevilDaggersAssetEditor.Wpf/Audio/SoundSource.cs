using OpenAlBindings;
using OpenAlBindings.Enums;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public class SoundSource
{
	private float _pitch = 1;

	private uint _sourceId;

	public SoundSource(WaveFile waveFile)
	{
		WaveFile = waveFile;
		_sourceId = WaveFile.CreateSource();

		SetPitch();
	}

	public WaveFile WaveFile { get; }

	public float Pitch
	{
		get => _pitch;
		set
		{
			_pitch = value;
			SetPitch();
		}
	}

	public float Offset
	{
		get
		{
			Al.alGetSourcef(_sourceId, FloatSourceProperty.AL_SEC_OFFSET, out float offset);
			return offset;
		}
		set => Al.alSourcef(_sourceId, FloatSourceProperty.AL_SEC_OFFSET, value);
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
		if (State is SourceState.Paused or SourceState.Initial)
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
