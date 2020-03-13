using DevilDaggersAssetCore.Chunks;
using System.Collections.Generic;

namespace DevilDaggersAssetCore
{
	public static class BinaryFileUtils
	{
		public const int ModelHeaderByteCount = 10;
		public const int ShaderHeaderByteCount = 12;
		public const int TextureHeaderByteCount = 11;

		public static readonly List<ChunkInfo> ChunkInfos = new List<ChunkInfo>
		{
			new ChunkInfo(BinaryFileType.Dd, typeof(ModelChunk), new ushort[] { 0x01 }, ".obj", "Models"),
			new ChunkInfo(BinaryFileType.Dd, typeof(TextureChunk), new ushort[] { 0x02 }, ".png", "Textures"),
			new ChunkInfo(BinaryFileType.Dd | BinaryFileType.Core, typeof(ShaderChunk), new ushort[] { 0x10, 0x11 }, ".glsl", "Shaders"),
			new ChunkInfo(BinaryFileType.Audio, typeof(AudioChunk), new ushort[] { 0x20 }, ".wav", "Audio"),
			new ChunkInfo(BinaryFileType.Dd, typeof(ModelBindingChunk), new ushort[] { 0x80 }, ".txt", "Model Bindings"),
		};
	}
}