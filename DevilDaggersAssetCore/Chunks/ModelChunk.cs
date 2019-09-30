using DevilDaggersAssetCore.Headers;
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

		private class Vertex
		{
			public const int ByteCount = 32;

			public float[] Position { get; set; } = new float[3];
			public float[] Normal { get; set; } = new float[3];
			public float[] UV { get; set; } = new float[2];

			public static Vertex Create(byte[] buffer, int i)
			{
				return new Vertex
				{
					Position = new float[3]
					{
						BitConverter.ToSingle(buffer, i * ByteCount),
						BitConverter.ToSingle(buffer, i * ByteCount + 4),
						BitConverter.ToSingle(buffer, i * ByteCount + 8)
					},
					Normal = new float[3]
					{
						BitConverter.ToSingle(buffer, i * ByteCount + 12),
						BitConverter.ToSingle(buffer, i * ByteCount + 16),
						BitConverter.ToSingle(buffer, i * ByteCount + 20)
					},
					UV = new float[2]
					{
						BitConverter.ToSingle(buffer, i * ByteCount + 24),
						BitConverter.ToSingle(buffer, i * ByteCount + 28)
					}
				};
			}

			public byte[] ToByteArray()
			{
				byte[] bytes = new byte[ByteCount];
				System.Buffer.BlockCopy(BitConverter.GetBytes(Position[0]), 0, bytes, 0, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(Position[1]), 0, bytes, 4, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(Position[2]), 0, bytes, 8, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(Normal[0]), 0, bytes, 12, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(Normal[1]), 0, bytes, 16, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(Normal[2]), 0, bytes, 20, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(UV[0]), 0, bytes, 24, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(UV[1]), 0, bytes, 28, sizeof(float));
				return bytes;
			}
		}

		public ModelChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Compress(string path)
		{
			string[] lines = File.ReadAllLines(path);

			List<Vertex> vertices = new List<Vertex>();
			List<uint> indices = new List<uint>();

			int i = 0;
			while (i < lines.Length)
			{
				if (lines[i].StartsWith("v"))
				{
					string[] position = lines[i].Split(' ');
					string[] uv = lines[i + 1].Split(' ');
					string[] normal = lines[i + 2].Split(' ');
					vertices.Add(new Vertex
					{
						Position = new float[3]
						{
							float.Parse(position[1]),
							float.Parse(position[2]),
							float.Parse(position[3])
						},
						UV = new float[2]
						{
							float.Parse(uv[1]),
							float.Parse(uv[2])
						},
						Normal = new float[3]
						{
							float.Parse(normal[1]),
							float.Parse(normal[2]),
							float.Parse(normal[3])
						}
					});

					i += 3;
				}
				else if (lines[i].StartsWith("f"))
				{
					string[] indicesLine = lines[i].Split(' ');
					indices.Add(uint.Parse(indicesLine[1]) - 1);
					indices.Add(uint.Parse(indicesLine[2]) - 1);
					indices.Add(uint.Parse(indicesLine[3]) - 1);

					i++;
				}
				else i++;
			}

			byte[] headerBuffer = new byte[10]; // TODO: Get from ModelHeader.ByteCount but without creating an instance.
			System.Buffer.BlockCopy(BitConverter.GetBytes((uint)indices.Count), 0, headerBuffer, 0, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes((uint)vertices.Count), 0, headerBuffer, 4, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes((ushort)288), 0, headerBuffer, 8, sizeof(ushort));
			Header = new ModelHeader(headerBuffer);

			Buffer = new byte[vertices.Count * Vertex.ByteCount + indices.Count * sizeof(uint) + closures[Name].Length];
			for (int j = 0; j < vertices.Count; j++)
				System.Buffer.BlockCopy(vertices[j].ToByteArray(), 0, Buffer, j * Vertex.ByteCount, Vertex.ByteCount);
			for (int j = 0; j < indices.Count; j++)
				System.Buffer.BlockCopy(BitConverter.GetBytes(indices[j]), 0, Buffer, vertices.Count * Vertex.ByteCount + j * sizeof(uint), sizeof(uint));
			System.Buffer.BlockCopy(closures[Name], 0, Buffer, vertices.Count * Vertex.ByteCount + indices.Count * sizeof(uint), closures[Name].Length);

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;
		}

		public override IEnumerable<FileResult> Extract()
		{
			Vertex[] vertices = new Vertex[Header.VertexCount];
			uint[] indices = new uint[Header.IndexCount];

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = Vertex.Create(Buffer, i);

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