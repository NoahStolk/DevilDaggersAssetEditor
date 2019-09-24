using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ModelHeader : AbstractHeader
	{
		public override int ByteCount => 10;

		public uint IndexCount { get; }
		public uint VertexCount { get; }
		public ushort Unknown { get; }

		public ModelHeader(byte[] chunkBuffer)
			: base(chunkBuffer)
		{
			IndexCount = BitConverter.ToUInt32(Buffer, 0);
			VertexCount = BitConverter.ToUInt32(Buffer, 4);
			Unknown = BitConverter.ToUInt16(Buffer, 8);
		}
	}
}