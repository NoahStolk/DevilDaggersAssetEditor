using DevilDaggersAssetEditor.Data;
using DevilDaggersAssetEditor.Headers;
using DevilDaggersAssetEditor.Info;
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
	public class ModelChunk : AbstractHeaderedChunk<ModelHeader>
	{
		private static readonly Dictionary<string, byte[]> _closures;

		static ModelChunk()
		{
			using StreamReader sr = new StreamReader(Utils.GetContentStream("ModelClosures.json"));
			_closures = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(sr.ReadToEnd());
		}

		public ModelChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		private static float ParseVertexValue(string value)
			=> (float)double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);

		public override void MakeBinary(string path)
		{
			ReadObj(path, out List<Vector3> outPositions, out List<Vector2> outTexCoords, out List<Vector3> outNormals, out List<VertexReference> outVertices);

			int vertexCount = outPositions.Count;

			byte[] headerBuffer = new byte[ChunkInfo.Model.HeaderInfo.FixedSize.Value];
			Buf.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, headerBuffer, 0, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, headerBuffer, 4, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes((ushort)288), 0, headerBuffer, 8, sizeof(ushort));
			Header = new ModelHeader(headerBuffer);

			byte[] closure = _closures[Name];
			Buffer = new byte[vertexCount * Vertex.ByteCount + vertexCount * sizeof(uint) + closure.Length];
			for (int i = 0; i < vertexCount; i++)
			{
				Vertex vertex = new Vertex(outPositions[(int)outVertices[i].PositionReference - 1], outTexCoords[(int)outVertices[i].TexCoordReference - 1], outNormals[(int)outVertices[i].NormalReference - 1]);
				byte[] vertexBytes = vertex.ToByteArray();
				Buf.BlockCopy(vertexBytes, 0, Buffer, i * Vertex.ByteCount, Vertex.ByteCount);
			}

			for (int i = 0; i < vertexCount; i++)
				Buf.BlockCopy(BitConverter.GetBytes(outVertices[i].PositionReference - 1), 0, Buffer, vertexCount * Vertex.ByteCount + i * sizeof(uint), sizeof(uint));
			Buf.BlockCopy(closure, 0, Buffer, vertexCount * (Vertex.ByteCount + sizeof(uint)), closure.Length);

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;
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
				string identifier = values[0];

				switch (identifier)
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
							throw new NotImplementedException("Turning models consisting of NGons into binary data has not been implemented.");

						for (int j = 0; j < 3; j++)
						{
							string value = values[j + 1];

							if (value.Contains("/", StringComparison.InvariantCulture))
							{
								// f 1/2/3 4/5/6 7/8/9
								string[] references = value.Split('/');

								if (!uint.TryParse(references[0], out uint positionReference))
									throw new Exception($"Invalid vertex data in file '{Path.GetFileName(path)}' at line {i + 1}: Position value '{references[0]}' could not be parsed to a positive integral value ({typeof(uint).Name}).");
								if (!uint.TryParse(references[1], out uint texCoordReference))
									throw new Exception($"Invalid vertex data in file '{Path.GetFileName(path)}' at line {i + 1}: Tex coord value '{references[1]}' could not be parsed to a positive integral value ({typeof(uint).Name}).");
								if (!uint.TryParse(references[2], out uint normalReference))
									throw new Exception($"Invalid vertex data in file '{Path.GetFileName(path)}' at line {i + 1}: Normal value '{references[2]}' could not be parsed to a positive integral value ({typeof(uint).Name}).");

								vertices.Add(new VertexReference(positionReference, texCoordReference, normalReference));
							}
							else
							{
								// f 1 2 3
								vertices.Add(new VertexReference(uint.Parse(value, CultureInfo.InvariantCulture)));
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
			Vertex[] vertices = new Vertex[Header.VertexCount];
			uint[] indices = new uint[Header.IndexCount];

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = Vertex.CreateFromBuffer(Buffer, i);

			for (int i = 0; i < indices.Length; i++)
				indices[i] = BitConverter.ToUInt32(Buffer, vertices.Length * Vertex.ByteCount + i * sizeof(uint));

			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"# {Name}.obj\n");

			sb.AppendLine("# Vertex Attributes");
			StringBuilder v = new StringBuilder();
			StringBuilder vt = new StringBuilder();
			StringBuilder vn = new StringBuilder();
			for (uint i = 0; i < Header.VertexCount; ++i)
			{
				v.AppendLine($"v {vertices[i].Position.X} {vertices[i].Position.Y} {vertices[i].Position.Z}");
				vt.AppendLine($"vt {vertices[i].TexCoord.X} {vertices[i].TexCoord.Y}");
				vn.AppendLine($"vn {vertices[i].Normal.X} {vertices[i].Normal.Y} {vertices[i].Normal.Z}");
			}

			sb.Append(v);
			sb.Append(vt);
			sb.Append(vn);

			sb.AppendLine("\n# Triangles");
			for (uint i = 0; i < Header.IndexCount / 3; ++i)
			{
				VertexReference vertex1 = new VertexReference(indices[i * 3] + 1);
				VertexReference vertex2 = new VertexReference(indices[i * 3 + 1] + 1);
				VertexReference vertex3 = new VertexReference(indices[i * 3 + 2] + 1);
				sb.AppendLine($"f {vertex1} {vertex2} {vertex3}");
			}

			yield return new FileResult(Name, Encoding.Default.GetBytes(sb.ToString()));
		}
	}
}