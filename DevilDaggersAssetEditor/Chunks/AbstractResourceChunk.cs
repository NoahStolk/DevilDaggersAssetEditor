using DevilDaggersAssetEditor.BinaryFileHandlers;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetEditor.Chunks
{
	public abstract class AbstractResourceChunk : AbstractChunk
	{
		protected AbstractResourceChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size)
		{
		}

		public virtual void MakeBinary(string path)
		{
			Buffer = File.ReadAllBytes(path);
			Size = (uint)Buffer.Length;
		}

		public virtual IEnumerable<FileResult> ExtractBinary()
		{
			yield return new FileResult(Name, Buffer);
		}

		public override string ToString() => $"Type: {GetType().Name} | Name: {Name} | Size: {Size}";
	}
}