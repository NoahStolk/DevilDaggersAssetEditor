using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ModelHeader : AbstractHeader
	{
		public ModelHeader(byte[] chunkBuffer)
			: base(chunkBuffer)
		{
			IndexCount = BitConverter.ToUInt32(Buffer, 0);
			VertexCount = BitConverter.ToUInt32(Buffer, 4);
			Unknown = BitConverter.ToUInt16(Buffer, 8);
		}

		public override int ByteCount => 10;

		public uint IndexCount { get; set; }
		public uint VertexCount { get; set; }
		public ushort Unknown { get; set; }
	}
}