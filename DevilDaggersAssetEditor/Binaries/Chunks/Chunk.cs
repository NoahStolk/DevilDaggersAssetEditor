using DevilDaggersCore.Mods;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetEditor.Binaries.Chunks
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

		public string Name { get; private set; }
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

		public void Enable()
			=> Name = Name.ToLower();

		public void Disable()
			=> Name = Name.ToUpper();

		public virtual bool IsBinaryEqual(Chunk? otherChunk, out string? diffReason)
		{
			if (otherChunk == null)
			{
				diffReason = "Other chunk is not present.";
				return false;
			}

			if (Buffer.Length != otherChunk.Buffer.Length)
			{
				diffReason = $"Chunks do not have the same length ({Buffer.Length} - {otherChunk.Buffer.Length}).";
				return false;
			}

			for (int i = 0; i < Buffer.Length; i++)
			{
				if (Buffer[i] != otherChunk.Buffer[i])
				{
					diffReason = $"Bytes at position {i} do not match (0x{Buffer[i]:X} - 0x{otherChunk.Buffer[i]:X}).";
					return false;
				}
			}

			diffReason = null;
			return true;
		}
	}
}
