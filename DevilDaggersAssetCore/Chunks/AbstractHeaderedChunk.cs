using DevilDaggersAssetCore.Headers;
using System;

namespace DevilDaggersAssetCore.Chunks
{
	public abstract class AbstractHeaderedChunk<T> : AbstractChunk where T : AbstractHeader
	{
		public T Header { get; set; }

		protected AbstractHeaderedChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Init(byte[] buffer)
		{
			Header = Activator.CreateInstance(typeof(T), buffer) as T;

			Buffer = new byte[buffer.Length - Header.ByteCount];
			System.Buffer.BlockCopy(buffer, Header.ByteCount, Buffer, 0, Buffer.Length);
		}
	}
}