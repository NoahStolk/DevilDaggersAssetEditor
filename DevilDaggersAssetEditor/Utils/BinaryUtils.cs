using System.Text;

namespace DevilDaggersAssetEditor.Utils
{
	public static class BinaryUtils
	{
		/// <summary>
		/// Reads a null terminated string from a buffer and returns it as a string object (excluding the null terminator itself).
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">The starting offset to start reading from within the buffer.</param>
		/// <returns>The null terminated string.</returns>
		public static string ReadNullTerminatedString(byte[] buffer, int offset)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = offset; i < buffer.Length; i++)
			{
				char c = (char)buffer[i];
				if (c == '\0')
					return sb.ToString();
				sb.Append(c);
			}

			throw new($"Null terminator not observed in buffer with length {buffer.Length} starting from offset {offset}.");
		}
	}
}
