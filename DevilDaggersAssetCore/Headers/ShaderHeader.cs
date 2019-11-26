using System;

namespace DevilDaggersAssetCore.Headers
{
	public class ShaderHeader : AbstractHeader
	{
		public override int ByteCount => BinaryFileUtils.ShaderHeaderByteCount;

		public uint NameLength { get; }
		public uint VertexSize { get; }
		public uint FragmentSize { get; }

		public ShaderHeader(byte[] chunkBuffer)
			: base(chunkBuffer)
		{
			NameLength = BitConverter.ToUInt32(Buffer, 0);
			VertexSize = BitConverter.ToUInt32(Buffer, 4);
			FragmentSize = BitConverter.ToUInt32(Buffer, 8);
		}
	}
}