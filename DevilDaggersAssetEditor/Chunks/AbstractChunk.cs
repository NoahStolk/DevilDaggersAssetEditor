namespace DevilDaggersAssetEditor.Chunks
{
	public abstract class AbstractChunk
	{
		public string Name { get; set; }
		public uint StartOffset { get; set; }
		public uint Size { get; set; }

		public byte[] Buffer { get; set; }

		protected AbstractChunk(string name, uint startOffset, uint size)
		{
			Name = name;
			StartOffset = startOffset;
			Size = size;
		}
	}
}