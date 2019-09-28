using System;

namespace DevilDaggersAssetCore.Headers
{
	public class TextureHeader : AbstractHeader
	{
		public override int ByteCount => 11;

		public ushort Unknown { get; } // Color format?
		public uint Width { get; }
		public uint Height { get; }
		public byte Mipmaps { get; } // Mipmaps can be calculated by performing binary logarithm (Log2n) on the smallest dimension of the image and casting it to a byte (which is essentially equivalent to Math.Floor).

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