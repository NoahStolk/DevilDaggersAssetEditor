using Buf = System.Buffer;

namespace DevilDaggersAssetCore.Headers
{
	public abstract class AbstractResourceHeader : AbstractHeader
	{
		protected AbstractResourceHeader(byte[] headerBuffer)
		{
			Buffer = new byte[ByteCount];
			Buf.BlockCopy(headerBuffer, 0, Buffer, 0, ByteCount);
		}
	}
}