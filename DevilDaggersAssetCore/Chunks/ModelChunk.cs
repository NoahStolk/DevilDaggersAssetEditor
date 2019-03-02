using DevilDaggersAssetCore.Headers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevilDaggersAssetCore.Chunks
{
	public class ModelChunk : AbstractHeaderedChunk<ModelHeader>
	{
		public override string FileExtension => ".obj";

		private class Vertex
		{
			public const int Bytes = 32;

			public float[] Position { get; set; } = new float[3];
			public float[] Normal { get; set; } = new float[3];
			public float[] UV { get; set; } = new float[2];
		}

		public ModelChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override IEnumerable<FileResult> Extract()
		{
			Vertex[] vertices = new Vertex[Header.VertexCount];
			uint[] indices = new uint[Header.IndexCount];

			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = new Vertex
				{
					Position = new float[3]
					{
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes),
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 4),
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 8)
					},
					Normal = new float[3]
					{
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 12),
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 16),
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 20)
					},
					UV = new float[2]
					{
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 24),
						BitConverter.ToSingle(Buffer, i * Vertex.Bytes + 28)
					}
				};
			}

			for (int i = 0; i < indices.Length; i++)
			{
				indices[i] = BitConverter.ToUInt32(Buffer, vertices.Length * Vertex.Bytes + i * sizeof(uint));
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {Name}.obj\n");

			sb.AppendLine("# Vertex Attributes");
			for (uint i = 0; i < Header.VertexCount; ++i)
			{
				sb.AppendLine($"v {vertices[i].Position[0]} {vertices[i].Position[1]} {vertices[i].Position[2]}");
				sb.AppendLine($"vt {vertices[i].UV[0]} {vertices[i].UV[1]}");
				sb.AppendLine($"vn {vertices[i].Normal[0]} {vertices[i].Normal[1]} {vertices[i].Normal[2]}");
			}

			sb.AppendLine("\n# Triangles");
			for (uint i = 0; i < Header.IndexCount / 3; ++i)
			{
				sb.AppendLine($"f {indices[i * 3] + 1} {indices[i * 3 + 1] + 1} {indices[i * 3 + 2] + 1}");
			}

			yield return new FileResult(Name, Encoding.Default.GetBytes(sb.ToString()));
		}
	}
}