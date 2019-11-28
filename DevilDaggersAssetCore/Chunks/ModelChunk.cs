using DevilDaggersAssetCore.Headers;
using NetBase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevilDaggersAssetCore.Chunks
{
	public class ModelChunk : AbstractHeaderedChunk<ModelHeader>
	{
		private static readonly Dictionary<string, byte[]> closures;

		static ModelChunk()
		{
			using StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream("DevilDaggersAssetCore.Content.ModelClosures.json"));
			closures = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(sr.ReadToEnd());
		}

		public ModelChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Compress(string path)
		{
			string text = File.ReadAllText(path);
			int vertexLines = text.CountOccurrences("v ");
			int indexLines = text.CountOccurrences("f ");

			string[] lines = text.Split('\n');

			Vertex[] vertices = new Vertex[vertexLines];
			uint[] indices = new uint[indexLines * 3];

			int vCount = 0;
			int vtCount = 0;
			int vnCount = 0;
			int fCount = 0;

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				string[] values = line.Split(' ');
				string identifier = values[0];

				switch (identifier)
				{
					case "v":
						vertices[vCount].Position = new float[3]
						{
							float.Parse(values[1]),
							float.Parse(values[2]),
							float.Parse(values[3])
						};
						vCount++;
						break;
					case "vt":
						vertices[vtCount].UV = new float[2]
						{
							float.Parse(values[1]),
							float.Parse(values[2])
						};
						vtCount++;
						break;
					case "vn":
						vertices[vnCount].Normal = new float[3]
						{
							float.Parse(values[1]),
							float.Parse(values[2]),
							float.Parse(values[3])
						};
						vnCount++;
						break;
					case "f":
						for (int j = 0; j < 3; j++)
						{
							string value = values[j + 1];

							if (value.Contains("/"))
								value = value.Substring(0, value.IndexOf('/'));

							indices[fCount++] = uint.Parse(value) - 1;
						}
						break;
				}
			}

			byte[] headerBuffer = new byte[BinaryFileUtils.ModelHeaderByteCount];
			System.Buffer.BlockCopy(BitConverter.GetBytes((uint)indices.Length), 0, headerBuffer, 0, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes((uint)vertices.Length), 0, headerBuffer, 4, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes((ushort)288), 0, headerBuffer, 8, sizeof(ushort));
			Header = new ModelHeader(headerBuffer);

			Buffer = new byte[vertices.Length * Vertex.ByteCount + indices.Length * sizeof(uint) + closures[Name].Length];
			for (int j = 0; j < vertices.Length; j++)
				System.Buffer.BlockCopy(vertices[j].ToByteArray(), 0, Buffer, j * Vertex.ByteCount, Vertex.ByteCount);
			for (int j = 0; j < indices.Length; j++)
				System.Buffer.BlockCopy(BitConverter.GetBytes(indices[j]), 0, Buffer, vertices.Length * Vertex.ByteCount + j * sizeof(uint), sizeof(uint));
			System.Buffer.BlockCopy(closures[Name], 0, Buffer, vertices.Length * Vertex.ByteCount + indices.Length * sizeof(uint), closures[Name].Length);

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;
		}

		public override IEnumerable<FileResult> Extract()
		{
			Vertex[] vertices = new Vertex[Header.VertexCount];
			uint[] indices = new uint[Header.IndexCount];

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = new Vertex(Buffer, i);

			for (int i = 0; i < indices.Length; i++)
				indices[i] = BitConverter.ToUInt32(Buffer, vertices.Length * Vertex.ByteCount + i * sizeof(uint));

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