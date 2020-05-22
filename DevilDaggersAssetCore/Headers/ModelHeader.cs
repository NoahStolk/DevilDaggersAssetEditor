using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ModelHeader : AbstractResourceHeader
	{
		public override int ByteCount => BinaryFileUtils.ModelHeaderByteCount;

		public uint IndexCount { get; }
		public uint VertexCount { get; }
		public ushort Unknown { get; }

		public ModelHeader(byte[] headerBuffer)
			: base(headerBuffer)
		{
			IndexCount = BitConverter.ToUInt32(Buffer, 0);
			VertexCount = BitConverter.ToUInt32(Buffer, 4);
			Unknown = BitConverter.ToUInt16(Buffer, 8);
		}
	}
}