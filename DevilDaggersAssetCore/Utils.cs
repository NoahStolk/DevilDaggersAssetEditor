using DevilDaggersAssetCore.Chunks;
using System.Collections.Generic;

namespace DevilDaggersAssetCore
{
	public static class Utils
	{
		public static List<ChunkInfo> ChunkInfos { get; set; } = new List<ChunkInfo>
		{
			new ChunkInfo(typeof(ModelChunk), new ushort[] { 0x01 }, ".obj", "Models"),
			new ChunkInfo(typeof(TextureChunk), new ushort[] { 0x02 }, ".png", "Textures"),
			new ChunkInfo(typeof(ShaderChunk), new ushort[] { 0x10, 0x11 }, ".glsl", "Shaders"),
			new ChunkInfo(typeof(AudioChunk), new ushort[] { 0x20 }, ".wav", "Audio"),
			new ChunkInfo(typeof(ModelBindingChunk), new ushort[] { 0x80 }, ".txt", "Model Bindings"),
		};

		public static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
		{
			return a | b << 8 | c << 16 | d << 24;
		}
	}
}