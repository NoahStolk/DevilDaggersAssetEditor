using OpenAlBindings;
using OpenAlBindings.Enums;
using System.IO;
using System.Text;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public class Sound
{
	public Sound(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		using BinaryReader br = new(fs);
		string riffHeader = Encoding.Default.GetString(br.ReadBytes(4));
		if (riffHeader != "RIFF")
			throw new($"No 'RIFF' header for .wav file '{path}'.");

		_ = br.ReadInt32(); // Amount of bytes remaining at this point (after these 4).

		string format = Encoding.Default.GetString(br.ReadBytes(4));
		if (format != "WAVE")
			throw new($"No 'WAVE' header for .wav file '{path}'.");

		string fmtHeader = Encoding.Default.GetString(br.ReadBytes(4));
		if (fmtHeader != "fmt ")
			throw new($"No 'fmt ' header for .wav file '{path}'.");

		int fmtSize = br.ReadInt32();
		if (fmtSize != 16)
			throw new($"FMT data chunk size is not 16 for .wav file '{path}'.");

		short audioFormat = br.ReadInt16();
		if (audioFormat != 1)
			throw new($"Audio format is not 1 for .wav file '{path}'.");

		Channels = br.ReadInt16();
		SampleRate = br.ReadInt32();
		int byteRate = br.ReadInt32();
		short blockAlign = br.ReadInt16();
		BitsPerSample = br.ReadInt16();

		if (byteRate != SampleRate * Channels * BitsPerSample / 8)
			throw new($"Invalid byte rate for .wav file '{path}'.");
		if (blockAlign != Channels * BitsPerSample / 8)
			throw new($"Invalid block align for .wav file '{path}'.");

		string dataHeader = Encoding.Default.GetString(br.ReadBytes(4));
		if (dataHeader != "data")
			throw new($"No 'data' header for .wav file '{path}'.");

		Size = br.ReadInt32();
		Data = br.ReadBytes(Size);
	}

	public Sound(short channels, int sampleRate, short bitsPerSample, int size, byte[] data)
	{
		Channels = channels;
		SampleRate = sampleRate;
		BitsPerSample = bitsPerSample;
		Size = size;
		Data = data;
	}

	public short Channels { get; }
	public int SampleRate { get; }
	public short BitsPerSample { get; }
	public int Size { get; }
	public byte[] Data { get; }

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
