using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ShaderHeader : AbstractHeader
	{
		public ShaderHeader(byte[] bytes)
		{
			NameLength = BitConverter.ToUInt32(bytes, 0);
			VertexSize = BitConverter.ToUInt32(bytes, 4);
			FragmentSize = BitConverter.ToUInt32(bytes, 8);
		}

		public override int ByteCount => 12;

		public uint NameLength { get; set; }
		public uint VertexSize { get; set; }
		public uint FragmentSize { get; set; }
	}
}