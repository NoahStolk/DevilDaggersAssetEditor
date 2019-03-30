using DevilDaggersAssetCore.Headers;
using System.Collections.Generic;

namespace DevilDaggersAssetCore.Chunks
{
	public class ShaderChunk : AbstractHeaderedChunk<ShaderHeader>
	{
		public ShaderChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override IEnumerable<FileResult> Extract()
		{
			byte[] vertexBuffer = new byte[Header.VertexSize];
			System.Buffer.BlockCopy(Buffer, Name.Length, vertexBuffer, 0, (int)Header.VertexSize);
			yield return new FileResult($"{Name}_vertex", vertexBuffer);

			byte[] fragmentBuffer = new byte[Header.FragmentSize];
			System.Buffer.BlockCopy(Buffer, Name.Length + (int)Header.VertexSize, fragmentBuffer, 0, (int)Header.FragmentSize);
			yield return new FileResult($"{Name}_fragment", fragmentBuffer);
		}
	}
}