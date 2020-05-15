using Buf = System.Buffer;

namespace DevilDaggersAssetCore.Headers
{
	public abstract class AbstractHeader
	{
		public abstract int ByteCount { get; }

		public byte[] Buffer { get; }

		protected AbstractHeader(byte[] chunkBuffer)
		{
			Buffer = new byte[ByteCount];
			Buf.BlockCopy(chunkBuffer, 0, Buffer, 0, ByteCount);
		}
	}
}