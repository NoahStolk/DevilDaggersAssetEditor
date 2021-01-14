using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ModelChunk : ResourceChunk
	{
		private static readonly Dictionary<string, byte[]> _closures = GetClosures();

		public ModelChunk(string name, uint startOffset, uint size)
			: base(AssetType.Model, name, startOffset, size)
		{
		}

		private static Dictionary<string, byte[]> GetClosures()
		{
			using StreamReader sr = new StreamReader(AssemblyUtils.GetContentStream("ModelClosures.json"));
			return JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(sr.ReadToEnd());
		}

		private static float ParseVertexValue(string value)
			=> (float)double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);

		public override void MakeBinary(string path)
		{
			ReadObj(path, out List<Vector3> outPositions, out List<Vector2> outTexCoords, out List<Vector3> outNormals, out List<VertexReference> outVertices);

			int vertexCount = outPositions.Count;

			byte[] closure = _closures[Name];
			Buffer = new byte[10 + vertexCount * Vertex.ByteCount + vertexCount * sizeof(uint) + closure.Length];

			Buf.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, Buffer, 0, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, Buffer, 4, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes((ushort)288), 0, Buffer, 8, sizeof(ushort));

			for (int i = 0; i < vertexCount; i++)
			{
				Vertex vertex = new Vertex(outPositions[(int)outVertices[i].PositionReference - 1], outTexCoords[(int)outVertices[i].TexCoordReference - 1], outNormals[(int)outVertices[i].NormalReference - 1]);
				byte[] vertexBytes = vertex.ToByteArray();
				Buf.BlockCopy(vertexBytes, 0, Buffer, 10 + i * Vertex.ByteCount, Vertex.ByteCount);
			}

			for (int i = 0; i < vertexCount; i++)
				Buf.BlockCopy(BitConverter.GetBytes(outVertices[i].PositionReference - 1), 0, Buffer, 10 + vertexCount * Vertex.ByteCount + i * sizeof(uint), sizeof(uint));
			Buf.BlockCopy(closure, 0, Buffer, 10 + vertexCount * (Vertex.ByteCount + sizeof(uint)), closure.Length);

			Size = (uint)Buffer.Length;
		}

		public static void ReadObj(string path, out List<Vector3> outPositions, out List<Vector2> outTexCoords, out List<Vector3> outNormals, out List<VertexReference> outVertices)
		{
			string text = File.ReadAllText(path);
			string[] lines = text.Split('\n');

			List<Vector3> positions = new List<Vector3>();
			List<Vector2> texCoords = new List<Vector2>();
			List<Vector3> normals = new List<Vector3>();
			List<VertexReference> vertices = new List<VertexReference>();

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				string[] values = line.Split(' ');

				switch (values[0])
				{
					case "v":
						positions.Add(new Vector3(ParseVertexValue(values[1]), ParseVertexValue(values[2]), ParseVertexValue(values[3])));
						break;
					case "vt":
						texCoords.Add(new Vector2(ParseVertexValue(values[1]), ParseVertexValue(values[2])));
						break;
					case "vn":
						normals.Add(new Vector3(ParseVertexValue(values[1]), ParseVertexValue(values[2]), ParseVertexValue(values[3])));
						break;
					case "f":
						// Compatible with:
						// f 1 2 3
						// f 1/2/3 4/5/6 7/8/9
						// f 1/2/3 4/5/6 7/8/9 10/11/12
						if (values.Length > 5)
							throw new NotSupportedException("Turning models consisting of NGons into binary data is not supported.");

						for (int j = 0; j < 3; j++)
						{
							string value = values[j + 1];

							string baseErrorMessage = $"Invalid vertex data in file '{Path.GetFileName(path)}' at line {i + 1}:";

							if (value.Contains("/", StringComparison.InvariantCulture))
							{
								// f 1/2/3 4/5/6 7/8/9
								string[] references = value.Split('/');

								if (string.IsNullOrWhiteSpace(references[0]))
									throw new($"{baseErrorMessage} Empty position value found. This probably means your model file is corrupted.");
								if (string.IsNullOrWhiteSpace(references[1]))
									throw new($"{baseErrorMessage} Empty texture coordinate value found. Make sure to export your texture (UV) coordinates.");
								if (string.IsNullOrWhiteSpace(references[2]))
									throw new($"{baseErrorMessage} Empty normal value found. Make sure to export your normals.");

								if (!uint.TryParse(references[0], out uint positionReference))
									throw new($"{baseErrorMessage} Position value '{references[0]}' could not be parsed to a positive integral value ({typeof(uint).Name}).");
								if (!uint.TryParse(references[1], out uint texCoordReference))
									throw new($"{baseErrorMessage} Texture coordinate value '{references[1]}' could not be parsed to a positive integral value ({typeof(uint).Name}).");
								if (!uint.TryParse(references[2], out uint normalReference))
									throw new($"{baseErrorMessage} Normal value '{references[2]}' could not be parsed to a positive integral value ({typeof(uint).Name}).");

								vertices.Add(new VertexReference(positionReference, texCoordReference, normalReference));
							}
							else
							{
								// f 1 2 3
								if (string.IsNullOrWhiteSpace(value))
									throw new($"{baseErrorMessage} No vertex value found. This probably means your model file is corrupted.");
								if (!uint.TryParse(value, out uint unifiedValue))
									throw new($"{baseErrorMessage} Value '{value}' could not be parsed to a positive integral value ({typeof(uint).Name}).");

								vertices.Add(new VertexReference(unifiedValue));
							}
						}

						// If there are 4 vertices, we're dealing with quads. Convert quads by making a second triangle (CDA).
						if (values.Length == 5)
						{
							for (int j = 2; j < 5; j++)
							{
								int k = j;
								if (j > 3)
									k -= 4;
								string value = values[k + 1];
								if (value.Contains("/", StringComparison.InvariantCulture))
								{
									// f 1/2/3 4/5/6 7/8/9
									string[] references = value.Split('/');

									vertices.Add(new VertexReference(uint.Parse(references[0], CultureInfo.InvariantCulture), uint.Parse(references[1], CultureInfo.InvariantCulture), uint.Parse(references[2], CultureInfo.InvariantCulture)));
								}
							}
						}

						break;
				}
			}

			outPositions = new List<Vector3>();
			outTexCoords = new List<Vector2>();
			outNormals = new List<Vector3>();
			outVertices = new List<VertexReference>();

			// Duplicate vertices as needed.
			for (uint i = 0; i < vertices.Count; i += 3)
			{
				// Three vertices make up one face.
				VertexReference vertex1 = vertices[(int)i];
				VertexReference vertex2 = vertices[(int)i + 1];
				VertexReference vertex3 = vertices[(int)i + 2];

				outPositions.Add(positions[(int)vertex1.PositionReference - 1]);
				outPositions.Add(positions[(int)vertex2.PositionReference - 1]);
				outPositions.Add(positions[(int)vertex3.PositionReference - 1]);

				outTexCoords.Add(texCoords[(int)vertex1.TexCoordReference - 1]);
				outTexCoords.Add(texCoords[(int)vertex2.TexCoordReference - 1]);
				outTexCoords.Add(texCoords[(int)vertex3.TexCoordReference - 1]);

				outNormals.Add(normals[(int)vertex1.NormalReference - 1]);
				outNormals.Add(normals[(int)vertex2.NormalReference - 1]);
				outNormals.Add(normals[(int)vertex3.NormalReference - 1]);

				VertexReference outVertex1 = new VertexReference(i + 1);
				VertexReference outVertex2 = new VertexReference(i + 2);
				VertexReference outVertex3 = new VertexReference(i + 3);

				outVertices.Add(outVertex1);
				outVertices.Add(outVertex2);
				outVertices.Add(outVertex3);
			}
		}

		public override IEnumerable<FileResult> ExtractBinary()
		{
			uint indexCount = BitConverter.ToUInt32(Buffer, 0);
			uint vertexCount = BitConverter.ToUInt32(Buffer, 4);

			Vertex[] vertices = new Vertex[vertexCount];
			uint[] indices = new uint[indexCount];

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = Vertex.CreateFromBuffer(Buffer, 10, i);

			for (int i = 0; i < indices.Length; i++)
				indices[i] = BitConverter.ToUInt32(Buffer, 10 + vertices.Length * Vertex.ByteCount + i * sizeof(uint));

			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {Name}.obj\n");

			sb.AppendLine("# Vertex Attributes");
			StringBuilder v = new StringBuilder();
			StringBuilder vt = new StringBuilder();
			StringBuilder vn = new StringBuilder();
			for (uint i = 0; i < vertexCount; ++i)
			{
				v.AppendLine($"v {vertices[i].Position.X} {vertices[i].Position.Y} {vertices[i].Position.Z}");
				vt.AppendLine($"vt {vertices[i].TexCoord.X} {vertices[i].TexCoord.Y}");
				vn.AppendLine($"vn {vertices[i].Normal.X} {vertices[i].Normal.Y} {vertices[i].Normal.Z}");
			}

			sb.Append(v);
			sb.Append(vt);
			sb.Append(vn);

			sb.AppendLine("\n# Triangles");
			for (uint i = 0; i < indexCount / 3; ++i)
			{
				VertexReference vertex1 = new VertexReference(indices[i * 3] + 1);
				VertexReference vertex2 = new VertexReference(indices[i * 3 + 1] + 1);
				VertexReference vertex3 = new VertexReference(indices[i * 3 + 2] + 1);
				sb.AppendLine($"f {vertex1} {vertex2} {vertex3}");
			}

			yield return new FileResult(Name, Encoding.Default.GetBytes(sb.ToString()));
		}

		private struct Vertex
		{
			public const int ByteCount = 32;

			public Vertex(Vector3 position, Vector2 texCoord, Vector3 normal)
			{
				Position = position;
				TexCoord = texCoord;
				Normal = normal;
			}

			public Vector3 Position { get; set; }
			public Vector2 TexCoord { get; set; }
			public Vector3 Normal { get; set; }

			public byte[] ToByteArray()
			{
				// TexCoord and Normal are swapped in the byte format for some reason.
				byte[] bytes = new byte[32];
				Buf.BlockCopy(BitConverter.GetBytes(Position.X), 0, bytes, 0, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(Position.Y), 0, bytes, 4, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(Position.Z), 0, bytes, 8, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(Normal.X), 0, bytes, 12, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(Normal.Y), 0, bytes, 16, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(Normal.Z), 0, bytes, 20, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(TexCoord.X), 0, bytes, 24, sizeof(float));
				Buf.BlockCopy(BitConverter.GetBytes(TexCoord.Y), 0, bytes, 28, sizeof(float));
				return bytes;
			}

			public static Vertex CreateFromBuffer(byte[] buffer, int offset, int vertexIndex)
			{
				Vector3 position = new Vector3(
					x: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount),
					y: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 4),
					z: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 8));
				Vector2 texCoord = new Vector2(
					x: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 24),
					y: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 28));
				Vector3 normal = new Vector3(
					x: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 12),
					y: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 16),
					z: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 20));
				return new Vertex(position, texCoord, normal);
			}
		}
	}
}
