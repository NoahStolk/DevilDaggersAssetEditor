using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ModelHeader : AbstractHeader
	{
		public ModelHeader(byte[] bytes)
		{
			IndexCount = BitConverter.ToUInt32(bytes, 0);
			VertexCount = BitConverter.ToUInt32(bytes, 4);
			Unknown = BitConverter.ToUInt16(bytes, 8);
		}

		public override int ByteCount => 10;

		public uint IndexCount { get; set; }
		public uint VertexCount { get; set; }
		public ushort Unknown { get; set; }
	}
}