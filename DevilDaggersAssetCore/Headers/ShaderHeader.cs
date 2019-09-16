using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ShaderHeader : AbstractHeader
	{
		public ShaderHeader(byte[] chunkBuffer)
			: base(chunkBuffer)
		{
			NameLength = BitConverter.ToUInt32(Buffer, 0);
			VertexSize = BitConverter.ToUInt32(Buffer, 4);
			FragmentSize = BitConverter.ToUInt32(Buffer, 8);
		}

		public override int ByteCount => 12;

		public uint NameLength { get; set; }
		public uint VertexSize { get; set; }
		public uint FragmentSize { get; set; }
	}
}