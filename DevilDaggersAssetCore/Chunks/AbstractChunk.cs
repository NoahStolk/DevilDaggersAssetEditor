using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetCore.Chunks
{
	public abstract class AbstractChunk
	{
		public string Name { get; set; }
		public uint StartOffset { get; set; }
		public uint Size { get; set; }
		public uint Unknown { get; set; }

		public byte[] Buffer { get; set; }

		protected AbstractChunk(string name, uint startOffset, uint size, uint unknown)
		{
			Name = name;
			StartOffset = startOffset;
			Size = size;
			Unknown = unknown;
		}

		// Only overridden by AbstractHeaderedChunk to take header into account.
		public virtual void SetBuffer(byte[] buffer) => Buffer = buffer;

		// Only overridden by AbstractHeaderedChunk to take header into account.
		public virtual byte[] GetBuffer() => Buffer;

		public virtual void Compress(string path)
		{
			Buffer = File.ReadAllBytes(path);
			Size = (uint)Buffer.Length;
		}

		public virtual IEnumerable<FileResult> Extract()
		{
			yield return new FileResult(Name, Buffer);
		}

		public override string ToString() => $"Type: {GetType().Name} | Name: {Name} | Size: {Size}";
	}
}