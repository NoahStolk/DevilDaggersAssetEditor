using OpenAlBindings;
using OpenAlBindings.Enums;
using System;
using System.Numerics;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public sealed class AudioEngine
{
	private static readonly Lazy<AudioEngine> _lazy = new(() => new());

	private AudioEngine()
	{
	}

	public static AudioEngine Instance => _lazy.Value;

	public static Vector3 ListenerPosition
	{
		get
		{
			Al.alGetListener3f(FloatSourceProperty.AL_POSITION, out float x, out float y, out float z);
			return new(x, y, z);
		}
		set => Al.alListenerfv(FloatSourceProperty.AL_POSITION, new[] { value.X, value.Y, value.Z });
	}

	public static Vector3 ListenerVelocity
	{
		get
		{
			Al.alGetListener3f(FloatSourceProperty.AL_VELOCITY, out float x, out float y, out float z);
			return new(x, y, z);
		}
		set => Al.alListenerfv(FloatSourceProperty.AL_VELOCITY, new[] { value.X, value.Y, value.Z });
	}

	public static Orientation ListenerOrientation
	{
		get
		{
			float[] values = new float[6];
			Al.alGetListenerfv(FloatSourceProperty.AL_ORIENTATION, values);
			return new()
			{
				At = new() { X = values[0], Y = values[1], Z = values[2] },
				Up = new() { X = values[3], Y = values[4], Z = values[5] },
			};
		}
		set => Al.alListenerfv(FloatSourceProperty.AL_ORIENTATION, new[] { value.At.X, value.At.Y, value.At.Z, value.Up.X, value.Up.Y, value.Up.Z });
	}

	public static void Initialize()
	{
		if (OpenAlDeviceHelper.PlaybackDevices.Length == 0)
			throw new("No devices found.");

		PlaybackDevice device = OpenAlDeviceHelper.PlaybackDevices[0];
		device.MakeCurrent();

		ListenerOrientation = new()
		{
			At = new() { X = 0.0f, Y = 0.0f, Z = 1.0f },
			Up = new() { X = 0.0f, Y = 1.0f, Z = 0.0f },
		};
	}
}
