namespace DevilDaggersAssetCore
{
	public static class Utils
	{
		public const ushort ChunkModel = 0x01;
		public const ushort ChunkTexture = 0x02;
		public const ushort ChunkShaderVertex = 0x10;
		public const ushort ChunkShaderFragment = 0x11;
		public const ushort ChunkAudio = 0x20;
		public const ushort ChunkModelBinding = 0x80;

		public static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
		{
			return a | b << 8 | c << 16 | d << 24;
		}
	}
}