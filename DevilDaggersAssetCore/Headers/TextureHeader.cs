using System;

namespace DevilDaggersAssetCore.Headers
{
	public class TextureHeader : AbstractHeader
	{
		public override int ByteCount => 11;

		public ushort Unknown { get; } // Color format?
		public uint Width { get; }
		public uint Height { get; }
		public byte Mipmaps { get; }

		public TextureHeader(byte[] chunkBuffer)
			: base(chunkBuffer)
		{
			Unknown = BitConverter.ToUInt16(Buffer, 0);
			Width = BitConverter.ToUInt32(Buffer, 2);
			Height = BitConverter.ToUInt32(Buffer, 6);
			Mipmaps = Buffer[10];
		}
	}
}