using DevilDaggersAssetCore.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevilDaggersAssetCore.BinaryFileHandlers
{
	public abstract class AbstractBinaryFileHandler
	{
		public BinaryFileType BinaryFileType { get; }

		protected AbstractBinaryFileHandler(BinaryFileType binaryFileType)
		{
			BinaryFileType = binaryFileType;
		}

		public abstract void Compress(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription);

		public abstract void Extract(string inputPath, string outputPath, Progress<float> progress, Progress<string> progressDescription);

		/// <summary>
		/// Reads a null terminated string from a buffer and returns it as a string object (excluding the null terminator itself).
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">The starting offset to start reading from within the buffer.</param>
		/// <returns>The null terminated string.</returns>
		protected string ReadNullTerminatedString(byte[] buffer, int offset)
		{
			if (offset >= buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(offset), $"Parameter '{nameof(offset)}' was out of range. Offset: {offset}, Buffer size: {buffer.Length}");

			string str = Encoding.UTF8.GetString(buffer);

			if (!str.Substring(offset).Contains("\0"))
				throw new Exception($"Null terminator not observed in buffer with length {buffer.Length} starting from offset {offset}.");

			return str.Substring(offset, str.IndexOf('\0'));
		}
	}
}