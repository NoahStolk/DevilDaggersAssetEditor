using System;
using System.Runtime.Serialization;

namespace DevilDaggersAssetEditor.Wpf.Audio;

[Serializable]
public class WaveException : Exception
{
	public WaveException()
	{
	}

	public WaveException(string? message)
		: base(message)
	{
	}

	public WaveException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}

	protected WaveException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
