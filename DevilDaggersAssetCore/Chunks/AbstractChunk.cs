using System.Collections.Generic;

namespace DevilDaggersAssetCore.Chunks
{
	public abstract class AbstractChunk
	{
		public abstract string FileExtension { get; }
		public abstract string FolderName { get; }

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

		public virtual IEnumerable<FileResult> Extract()
		{
			yield return new FileResult(Name, Buffer);
		}

		public override string ToString()
		{
			return $"{Name}\t{Size}\n{Buffer}";
		}
	}
}