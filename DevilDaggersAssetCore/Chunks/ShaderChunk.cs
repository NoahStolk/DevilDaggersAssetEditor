using DevilDaggersAssetCore.Headers;
using System;

namespace DevilDaggersAssetCore.Chunks
{
	public class ShaderChunk : AbstractHeaderedChunk<ShaderHeader>
	{
		public override string FileExtension => ShaderType == ShaderType.Vertex ? "_vertex.glsl" : ShaderType == ShaderType.Fragment ? "_fragment.glsl" : throw new Exception($"Unknown shader type: {ShaderType}");

		public ShaderType ShaderType { get; set; }

		public ShaderChunk(string name, uint startOffset, uint size, uint unknown, ShaderType shaderType)
			: base(name, startOffset, size, unknown)
		{
			ShaderType = shaderType;
		}

		public override bool TryExtract(out byte[] result)
		{
			int offset = 0;
			uint size = 0;

			switch (ShaderType)
			{
				case ShaderType.Vertex:
					offset = Name.Length;
					size = Header.VertexSize;
					break;
				case ShaderType.Fragment:
				default:
					offset = Name.Length + (int)Header.VertexSize;
					size = Header.FragmentSize;
					break;
			}

			result = new byte[size];
			System.Buffer.BlockCopy(Buffer, offset, result, 0, (int)size);

			return true;
		}
	}
}