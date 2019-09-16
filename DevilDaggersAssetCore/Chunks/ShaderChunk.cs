using DevilDaggersAssetCore.Headers;
using System;
using System.Collections.Generic;

namespace DevilDaggersAssetCore.Chunks
{
	public class ShaderChunk : AbstractHeaderedChunk<ShaderHeader>
	{
		public ShaderChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override string ChunkNameToFileName(int i = 0)
		{
			return i switch
			{
				0 => $"{Name}_vertex",
				1 => $"{Name}_fragment",
				_ => throw new Exception($"No file name for index {i}."),
			};
		}

		public override string FileNameToChunkName(int i = 0)
		{
			return i switch
			{
				0 => Name.Replace("_vertex", ""),
				1 => Name.Replace("_fragment", ""),
				_ => throw new Exception($"No chunk name for index {i}."),
			};
		}

		public override IEnumerable<FileResult> ToFileResult()
		{
			byte[] vertexBuffer = new byte[Header.VertexSize];
			System.Buffer.BlockCopy(Buffer, Name.Length, vertexBuffer, 0, (int)Header.VertexSize);
			yield return new FileResult(ChunkNameToFileName(0), vertexBuffer);

			byte[] fragmentBuffer = new byte[Header.FragmentSize];
			System.Buffer.BlockCopy(Buffer, Name.Length + (int)Header.VertexSize, fragmentBuffer, 0, (int)Header.FragmentSize);
			yield return new FileResult(ChunkNameToFileName(1), fragmentBuffer);
		}
	}
}