using DevilDaggersAssetCore.Headers;
using System;

namespace DevilDaggersAssetCore.Chunks
{
	public abstract class AbstractHeaderedChunk<THeader> : AbstractChunk
		where THeader : AbstractHeader
	{
		public THeader Header { get; set; }

		protected AbstractHeaderedChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void SetBuffer(byte[] buffer)
		{
			Header = Activator.CreateInstance(typeof(THeader), buffer) as THeader;

			Buffer = new byte[buffer.Length - Header.ByteCount];
			System.Buffer.BlockCopy(buffer, Header.ByteCount, Buffer, 0, Buffer.Length);
		}

		public override byte[] GetBuffer()
		{
			byte[] buffer = new byte[Header.ByteCount + Buffer.Length];

			System.Buffer.BlockCopy(Header.Buffer, 0, buffer, 0, Header.ByteCount);
			System.Buffer.BlockCopy(Buffer, 0, buffer, Header.ByteCount, Buffer.Length);

			return buffer;
		}
	}
}