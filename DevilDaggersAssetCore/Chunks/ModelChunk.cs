using DevilDaggersAssetCore.Headers;

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

		//public override bool TryExtract(out byte[] result)
		//{
		//	Vertex[] vertices = new Vertex[Buffer.Length];
		//	uint[] indices = (uint)(vertices + Vertex.Bytes * m_header->m_vertexCount);

		//	result << "# " << getName() << ".obj" << std::endl << std::endl;

		//	output << "# Vertex Attributes" << std::endl;
		//	for (uint v = 0; v < m_header->m_vertexCount; ++v)
		//	{
		//		output << "v " << vertices[v].Position[0] << " " << vertices[v].Position[1] << " " << vertices[v].Position[2] << std::endl;
		//		output << "vt " << vertices[v].UV[0] << " " << vertices[v].UV[1] << std::endl;
		//		output << "vn " << vertices[v].Normal[0] << " " << vertices[v].Normal[1] << " " << vertices[v].Normal[2] << std::endl;

		//	}

		//	output << std::endl << "# Triangles" << std::endl;
		//	for (uint32_t triangle = 0; triangle < m_header->m_indexCount / 3; ++triangle)
		//	{
		//		output << "f " << indices[triangle * 3] + 1 << " " << indices[triangle * 3 + 1] + 1 << " " << indices[triangle * 3 + 2] + 1 << std::endl;
		//	}
		//	return true;
		//}
	}
}