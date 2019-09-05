using DevilDaggersAssetCore.Chunks;
using System.Collections.Generic;

namespace DevilDaggersAssetCore
{
	public static class Utils
	{
		public static readonly ulong Magic1;
		public static readonly ulong Magic2;

		public static readonly List<ChunkInfo> ChunkInfos = new List<ChunkInfo>
		{
			new ChunkInfo(typeof(ModelChunk), new byte[] { 0x01 }, ".obj", "Models"),
			new ChunkInfo(typeof(TextureChunk), new byte[] { 0x02 }, ".png", "Textures"),
			new ChunkInfo(typeof(ShaderChunk), new byte[] { 0x10, 0x11 }, ".glsl", "Shaders"),
			new ChunkInfo(typeof(AudioChunk), new byte[] { 0x20 }, ".wav", "Audio"),
			new ChunkInfo(typeof(ModelBindingChunk), new byte[] { 0x80 }, ".txt", "Model Bindings"),
		};

		static Utils()
		{
			Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
			Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

			static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d) => a | b << 8 | c << 16 | d << 24;
		}
	}
}