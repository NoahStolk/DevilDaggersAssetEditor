﻿using DevilDaggersAssetEditor.Headers;
using System;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	public abstract class AbstractHeaderedChunk<THeader> : AbstractResourceChunk
		where THeader : AbstractHeader
	{
		protected AbstractHeaderedChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public THeader Header { get; set; }

		public override void SetBuffer(byte[] buffer)
		{
			Header = Activator.CreateInstance(typeof(THeader), buffer) as THeader;

			Buffer = new byte[buffer.Length - Header.ByteCount];
			Buf.BlockCopy(buffer, (int)Header.ByteCount, Buffer, 0, Buffer.Length);
		}

		public override byte[] GetBuffer()
		{
			byte[] buffer = new byte[Header.ByteCount + Buffer.Length];

			Buf.BlockCopy(Header.Buffer, 0, buffer, 0, (int)Header.ByteCount);
			Buf.BlockCopy(Buffer, 0, buffer, (int)Header.ByteCount, Buffer.Length);

			return buffer;
		}
	}
}