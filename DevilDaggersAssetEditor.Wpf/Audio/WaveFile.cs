using NoahStolk.OpenAlBindings;
using System.IO;
using System.Text;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public class WaveFile
{
	public WaveFile(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		using BinaryReader br = new(fs);
		string riffHeader = Encoding.Default.GetString(br.ReadBytes(4));
		if (riffHeader != "RIFF")
			throw new WaveFileException($"Expected 'RIFF' header (got '{riffHeader}') for .wav file '{path}'.");

		_ = br.ReadInt32(); // Amount of bytes remaining at this point (after these 4).

		string format = Encoding.Default.GetString(br.ReadBytes(4));
		if (format != "WAVE")
			throw new WaveFileException($"Expected 'WAVE' header (got '{format}') for .wav file '{path}'.");

		string fmtHeader = Encoding.Default.GetString(br.ReadBytes(4));
		if (fmtHeader != "fmt ")
			throw new WaveFileException($"Expected 'fmt ' header (got '{fmtHeader}') for .wav file '{path}'.");

		int fmtSize = br.ReadInt32();
		if (fmtSize != 16)
			throw new WaveFileException($"Expected FMT data chunk size to be 16 (got {fmtSize}) for .wav file '{path}'.");

		short audioFormat = br.ReadInt16();
		if (audioFormat != 1)
			throw new WaveFileException($"Expected audio format to be 1 (got {audioFormat}) for .wav file '{path}'.");

		Channels = br.ReadInt16();
		SampleRate = br.ReadInt32();
		int byteRate = br.ReadInt32();
		short blockAlign = br.ReadInt16();
		BitsPerSample = br.ReadInt16();

		int expectedByteRate = SampleRate * Channels * BitsPerSample / 8;
		int expectedBlockAlign = Channels * BitsPerSample / 8;
		if (byteRate != expectedByteRate)
			throw new WaveFileException($"Expected byte rate to be {expectedByteRate} (got {byteRate}) for .wav file '{path}'.");
		if (blockAlign != expectedBlockAlign)
			throw new WaveFileException($"Expected block align to be {expectedBlockAlign} (got {blockAlign}) for .wav file '{path}'.");

		const string data = nameof(data);
		string dataHeader;
		do
		{
			if (br.BaseStream.Position >= br.BaseStream.Length - (data.Length + sizeof(int)))
				throw new WaveFileException($"Could not find '{data}' header for .wav file '{path}'.");

			dataHeader = Encoding.Default.GetString(br.ReadBytes(4));
		}
		while (dataHeader != data);

		Size = br.ReadInt32();
		Data = br.ReadBytes(Size);
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

	public double GetLength()
	{
		int sampleCount = Size / (BitsPerSample / 8) / Channels;
		return sampleCount / (double)SampleRate;
	}
}
