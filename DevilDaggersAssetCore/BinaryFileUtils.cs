using DevilDaggersAssetCore.Chunks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevilDaggersAssetCore
{
	public static class BinaryFileUtils
	{
		public const int HeaderSize = 12;

		public static readonly ulong Magic1;
		public static readonly ulong Magic2;

		public static readonly List<ChunkInfo> ChunkInfos = new List<ChunkInfo>
		{
			new ChunkInfo(BinaryFileName.DD, typeof(ModelChunk), new ushort[] { 0x01 }, ".obj", "Models"),
			new ChunkInfo(BinaryFileName.DD, typeof(TextureChunk), new ushort[] { 0x02 }, ".png", "Textures"),
			new ChunkInfo(BinaryFileName.DD | BinaryFileName.Core, typeof(ShaderChunk), new ushort[] { 0x10, 0x11 }, ".glsl", "Shaders"),
			new ChunkInfo(BinaryFileName.Audio, typeof(AudioChunk), new ushort[] { 0x20 }, ".wav", "Audio"),
			new ChunkInfo(BinaryFileName.DD, typeof(ModelBindingChunk), new ushort[] { 0x80 }, ".txt", "Model Bindings"),
		};

		static BinaryFileUtils()
		{
			Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
			Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

			static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d) => a | b << 8 | c << 16 | d << 24;
		}

		/// <summary>
		/// Reads a null terminated string from a buffer and returns it as a string object (excluding the null terminator itself).
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">The starting offset to start reading from within the buffer.</param>
		/// <returns>The null terminated string.</returns>
		public static string ReadNullTerminatedString(byte[] buffer, int offset)
		{
			StringBuilder name = new StringBuilder();
			for (int i = 0; i < buffer.Length - offset; i++)
			{
				char c = (char)buffer[offset + i];
				if (c == '\0')
					return name.ToString();
				name.Append(c);
			}
			throw new Exception($"Null terminator not observed in buffer with length {buffer.Length} starting from offset {offset}.");
		}
	}
}