using System;
using System.Runtime.Serialization;

namespace DevilDaggersAssetEditor.Wpf.Audio;

[Serializable]
public class WaveFileException : Exception
{
	public WaveFileException()
	{
	}

	public WaveFileException(string? message)
		: base(message)
	{
	}

	public WaveFileException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}

	protected WaveFileException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
