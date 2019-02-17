namespace DevilDaggersAssetCore.Chunks
{
	public abstract class AbstractChunk
	{
		public abstract string FileExtension { get; }

		public string Name { get; set; }
		public uint StartOffset { get; set; }
		public uint Size { get; set; }
		public uint Unknown { get; set; }

		public byte[] Buffer { get; set; }

		public AbstractChunk(string name, uint startOffset, uint size, uint unknown)
		{
			Name = name;
			StartOffset = startOffset;
			Size = size;
			Unknown = unknown;
		}

		public virtual void Init(byte[] buffer)
		{
			Buffer = buffer;
		}

		public virtual bool TryExtract(out byte[] result)
		{
			result = Buffer;
			return true;
		}
	}
}