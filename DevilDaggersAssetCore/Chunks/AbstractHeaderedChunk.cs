using DevilDaggersAssetCore.Headers;
using System;

namespace DevilDaggersAssetCore.Chunks
{
	public abstract class AbstractHeaderedChunk<T> : AbstractChunk where T : AbstractHeader
	{
		public T Header { get; set; }

		public AbstractHeaderedChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Init(byte[] buffer)
		{
			Header = Activator.CreateInstance<T>();

			Header.Buffer = new byte[Header.ByteCount];
			System.Buffer.BlockCopy(buffer, 0, Header.Buffer, 0, Header.ByteCount);

			Buffer = new byte[buffer.Length - Header.Buffer.Length];
			System.Buffer.BlockCopy(buffer, Header.ByteCount, Buffer, 0, Buffer.Length);
		}
	}
}