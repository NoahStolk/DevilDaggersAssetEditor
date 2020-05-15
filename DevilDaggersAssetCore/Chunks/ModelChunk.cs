using DevilDaggersAssetCore.Data;
using DevilDaggersAssetCore.Headers;
using NetBase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DevilDaggersAssetCore.Chunks
{
	public class ModelChunk : AbstractHeaderedChunk<ModelHeader>
	{
		private static readonly Dictionary<string, byte[]> closures;

		public static readonly int VertexByteCount = 32;

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
			if (path.Contains("gib"))
				Debugger.Break();

			string text = File.ReadAllText(path);
			int vertexCount = text.CountOccurrences("v ");
			int indexCount = text.CountOccurrences("f ") * 3;
			string[] lines = text.Split('\n');

			List<Vector3> positions = new List<Vector3>();
			List<Vector2> texCoords = new List<Vector2>();
			List<Vector3> normals = new List<Vector3>();
			List<VertexReference> vertices = new List<VertexReference>();
			List<uint> indices = new List<uint>();

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				string[] values = line.Split(' ');
				string identifier = values[0];

				switch (identifier)
				{
					case "v":
						positions.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
						break;
					case "vt":
						texCoords.Add(new Vector2(float.Parse(values[1]), float.Parse(values[2])));
						break;
					case "vn":
						normals.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
						break;
					case "f":
						// Should be compatible with both:
						// f 1 2 3
						// f 1/2/3 4/5/6 7/8/9
						for (int j = 0; j < 3; j++)
						{
							string value = values[j + 1];

							if (value.Contains("/")) // f 1/2/3 4/5/6 7/8/9
							{
								string[] references = value.Split('/');

								vertices.Add(new VertexReference(int.Parse(references[0]), int.Parse(references[1]), int.Parse(references[2])));
								indices.Add(uint.Parse(references[0]) - 1); // Not sure which reference... Or if this is even right at all.
							}
							else // f 1 2 3
							{
								vertices.Add(new VertexReference(int.Parse(value)));
								indices.Add(uint.Parse(value) - 1);
							}
						}
						break;
				}
			}

			byte[] headerBuffer = new byte[BinaryFileUtils.ModelHeaderByteCount];
			System.Buffer.BlockCopy(BitConverter.GetBytes((uint)indexCount), 0, headerBuffer, 0, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, headerBuffer, 4, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes((ushort)288), 0, headerBuffer, 8, sizeof(ushort));
			Header = new ModelHeader(headerBuffer);

			Buffer = new byte[vertexCount * VertexByteCount + indexCount * sizeof(uint) + closures[Name].Length];
			for (int j = 0; j < vertexCount; j++)
				System.Buffer.BlockCopy(ToByteArray(positions[vertices[j].PositionReference], texCoords[vertices[j].TexCoordReference], normals[vertices[j].NormalReference]), 0, Buffer, j * VertexByteCount, VertexByteCount);
			for (int j = 0; j < indexCount; j++)
				System.Buffer.BlockCopy(BitConverter.GetBytes(indices[j]), 0, Buffer, vertexCount * VertexByteCount + j * sizeof(uint), sizeof(uint));
			System.Buffer.BlockCopy(closures[Name], 0, Buffer, vertexCount * VertexByteCount + indexCount * sizeof(uint), closures[Name].Length);

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;

			static byte[] ToByteArray(Vector3 position, Vector2 texCoord, Vector3 normal)
			{
				byte[] bytes = new byte[32];
				System.Buffer.BlockCopy(BitConverter.GetBytes(position.X), 0, bytes, 0, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(position.Y), 0, bytes, 4, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(position.Z), 0, bytes, 8, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(normal.X), 0, bytes, 12, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(normal.Y), 0, bytes, 16, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(normal.Z), 0, bytes, 20, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(texCoord.X), 0, bytes, 24, sizeof(float));
				System.Buffer.BlockCopy(BitConverter.GetBytes(texCoord.Y), 0, bytes, 28, sizeof(float));
				return bytes;
			}
		}

		public override IEnumerable<FileResult> Extract()
		{
			(Vector3 position, Vector2 texCoord, Vector3 normal)[] vertices = new (Vector3 position, Vector2 texCoord, Vector3 normal)[Header.VertexCount];
			uint[] indices = new uint[Header.IndexCount];

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = VertexFromBuffer(Buffer, i);

			for (int i = 0; i < indices.Length; i++)
				indices[i] = BitConverter.ToUInt32(Buffer, vertices.Length * VertexByteCount + i * sizeof(uint));

			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {Name}.obj\n");

			sb.AppendLine("# Vertex Attributes");
			StringBuilder v = new StringBuilder();
			StringBuilder vt = new StringBuilder();
			StringBuilder vn = new StringBuilder();
			for (uint i = 0; i < Header.VertexCount; ++i)
			{
				v.AppendLine($"v {vertices[i].position.X} {vertices[i].position.Y} {vertices[i].position.Z}");
				vt.AppendLine($"vt {vertices[i].texCoord.X} {vertices[i].texCoord.Y}");
				vn.AppendLine($"vn {vertices[i].normal.X} {vertices[i].normal.Y} {vertices[i].normal.Z}");
			}

			sb.Append(v.ToString());
			sb.Append(vt.ToString());
			sb.Append(vn.ToString());

			sb.AppendLine("\n# Triangles");
			for (uint i = 0; i < Header.IndexCount / 3; ++i)
				sb.AppendLine($"f {Face(indices[i * 3] + 1)} {Face(indices[i * 3 + 1] + 1)} {Face(indices[i * 3 + 2] + 1)}");

			yield return new FileResult(Name, Encoding.Default.GetBytes(sb.ToString()));

			static string Face(uint face) => $"{face}/{face}/{face}";

			static (Vector3 position, Vector2 texCoord, Vector3 normal) VertexFromBuffer(byte[] buffer, int vertexIndex)
			{
				Vector3 position = new Vector3(
					x: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount),
					y: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 4),
					z: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 8));
				Vector2 texCoord = new Vector2(
					x: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 24),
					y: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 28));
				Vector3 normal = new Vector3(
					x: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 12),
					y: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 16),
					z: BitConverter.ToSingle(buffer, vertexIndex * VertexByteCount + 20));
				return (position, texCoord, normal);
			}
		}
	}
}