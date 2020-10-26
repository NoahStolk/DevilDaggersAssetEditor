using DevilDaggersAssetEditor.BinaryFileHandlers;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetEditor.Chunks
{
	public abstract class ResourceChunk
	{
		protected ResourceChunk(string name, uint startOffset, uint size)
		{
			Name = name;
			StartOffset = startOffset;
			Size = size;
		}

		public string Name { get; set; }
		public uint StartOffset { get; set; }
		public uint Size { get; set; }

		public byte[] Buffer { get; set; }

		public virtual void MakeBinary(string path)
		{
			Buffer = File.ReadAllBytes(path);
			Size = (uint)Buffer.Length;
		}

		public virtual IEnumerable<FileResult> ExtractBinary()
		{
			yield return new FileResult(Name, Buffer);
		}

		public override string ToString()
			=> $"Type: {GetType().Name} | Name: {Name} | Size: {Size}";
	}
}