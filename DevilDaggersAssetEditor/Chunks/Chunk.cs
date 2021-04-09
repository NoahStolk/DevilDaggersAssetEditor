using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetEditor.Chunks
{
	public class Chunk
	{
		public Chunk(AssetType assetType, string name, uint startOffset, uint size)
		{
			AssetType = assetType;
			Name = name;
			StartOffset = startOffset;
			Size = size;
		}

		public string Name { get; }
		public uint StartOffset { get; set; }
		public uint Size { get; set; }

		public AssetType AssetType { get; }

		public byte[] Buffer { get; set; } = null!;

		public virtual int HeaderSize { get; }

		public virtual void MakeBinary(string path)
		{
			Buffer = File.ReadAllBytes(path);
			Size = (uint)Buffer.Length;
		}

		public virtual IEnumerable<FileResult> ExtractBinary()
		{
			yield return new(Name, Buffer);
		}

		public override string ToString()
			=> $"Type: {AssetType} | Name: {Name} | Size: {Size}";
	}
}
