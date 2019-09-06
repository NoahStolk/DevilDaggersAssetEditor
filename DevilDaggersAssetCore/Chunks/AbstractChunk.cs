using System.Collections.Generic;

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

		public virtual string ChunkNameToFileName(int i = 0)
		{
			return Name;
		}

		public virtual string FileNameToChunkName(int i = 0)
		{
			return Name;
		}

		public virtual void Init(byte[] buffer)
		{
			Buffer = buffer;
		}

		public virtual IEnumerable<FileResult> Extract()
		{
			yield return new FileResult(ChunkNameToFileName(), Buffer);
		}

		public virtual byte[] Compress()
		{
			return Buffer;
		}

		public override string ToString()
		{
			return $"Type: {GetType().Name} | Name: {Name} | Size: {Size}";
		}
	}
}