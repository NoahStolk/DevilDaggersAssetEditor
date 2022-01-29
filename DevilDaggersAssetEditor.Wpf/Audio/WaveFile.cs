using NoahStolk.OpenAlBindings;
using NoahStolk.WaveParser;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public class WaveFile
{
	public WaveFile(string path)
	{
		WaveData waveData = new(File.ReadAllBytes(path));
		Channels = waveData.Channels;
		SampleRate = waveData.SampleRate;
		BitsPerSample = waveData.BitsPerSample;
		Data = waveData.Data;
		LengthInSeconds = waveData.LengthInSeconds;
	}

	public short Channels { get; }
	public int SampleRate { get; }
	public short BitsPerSample { get; }
	public byte[] Data { get; }
	public double LengthInSeconds { get; }

	public uint CreateSource()
	{
		uint[] sources = new uint[1];
		Al.alGenSources(1, sources);
		uint sourceId = sources[0];

		uint[] buffers = new uint[1];
		Al.alGenBuffers(1, buffers);
		uint bufferId = buffers[0];

		Al.alBufferData(bufferId, GetAudioFormat(), Data, Data.Length, (uint)SampleRate);
		Al.alSourceQueueBuffers(sourceId, 1, new[] { bufferId });

		return sourceId;
	}

	private AudioFormat GetAudioFormat()
	{
		bool stereo = Channels > 1;

		return BitsPerSample switch
		{
			16 => stereo ? AudioFormat.Stereo16Bit : AudioFormat.Mono16Bit,
			8 => stereo ? AudioFormat.Stereo8Bit : AudioFormat.Mono8Bit,
			_ => throw new($"Could not get audio format for wave with {BitsPerSample} samples."),
		};
	}
}
