using System;

namespace DevilDaggersAssetCore.Headers
{
	public class TextureHeader : AbstractHeader
	{
		public TextureHeader(byte[] chunkBuffer)
			: base(chunkBuffer)
		{
			Unknown = BitConverter.ToUInt16(Buffer, 0);
			Width = BitConverter.ToUInt32(Buffer, 2);
			Height = BitConverter.ToUInt32(Buffer, 6);
			Mipmaps = Buffer[10];
		}

		public override int ByteCount => 11;

		public ushort Unknown { get; set; } // Color format?
		public uint Width { get; set; }
		public uint Height { get; set; }
		public byte Mipmaps { get; set; }
	}
}