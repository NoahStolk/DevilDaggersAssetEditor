using System;

namespace DevilDaggersAssetCore.Headers
{
	public class TextureHeader : AbstractHeader
	{
		public TextureHeader(byte[] bytes)
		{
			Unknown = BitConverter.ToUInt16(bytes, 0);
			Width = BitConverter.ToUInt32(bytes, 2);
			Height = BitConverter.ToUInt32(bytes, 6);
			Mipmaps = bytes[10];
		}

		public override int ByteCount => 11;

		public ushort Unknown { get; set; } // Color format?
		public uint Width { get; set; }
		public uint Height { get; set; }
		public byte Mipmaps { get; set; }
	}
}