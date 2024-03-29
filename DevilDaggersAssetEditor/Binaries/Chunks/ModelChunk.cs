using DevilDaggersAssetEditor.Mods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Binaries.Chunks;

public class ModelChunk : Chunk
{
	public ModelChunk(string name, uint startOffset, uint size)
		: base(AssetType.Model, name, startOffset, size)
	{
	}

	public override int HeaderSize => 10;

	private static float ParseVertexValue(string value)
		=> (float)double.Parse(value, NumberStyles.Float);

	public override void MakeBinary(string path)
	{
		ReadObj(path, out List<Vector3> outPositions, out List<Vector2> outTexCoords, out List<Vector3> outNormals, out List<VertexReference> outVertices);

		int vertexCount = outPositions.Count;

		Buffer = new byte[HeaderSize + vertexCount * Vertex.ByteCount + vertexCount * sizeof(uint)];

		Buf.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, Buffer, 0, sizeof(uint));
		Buf.BlockCopy(BitConverter.GetBytes((uint)vertexCount), 0, Buffer, 4, sizeof(uint));
		Buf.BlockCopy(BitConverter.GetBytes((ushort)288), 0, Buffer, 8, sizeof(ushort));

		for (int i = 0; i < vertexCount; i++)
		{
			Vertex vertex = new(outPositions[(int)outVertices[i].PositionReference - 1], outTexCoords[(int)outVertices[i].TexCoordReference - 1], outNormals[(int)outVertices[i].NormalReference - 1]);
			byte[] vertexBytes = vertex.ToByteArray();
			Buf.BlockCopy(vertexBytes, 0, Buffer, HeaderSize + i * Vertex.ByteCount, Vertex.ByteCount);
		}

		for (int i = 0; i < vertexCount; i++)
			Buf.BlockCopy(BitConverter.GetBytes(outVertices[i].PositionReference - 1), 0, Buffer, HeaderSize + vertexCount * Vertex.ByteCount + i * sizeof(uint), sizeof(uint));

		Size = (uint)Buffer.Length;
	}

	public static void ReadObj(string path, out List<Vector3> outPositions, out List<Vector2> outTexCoords, out List<Vector3> outNormals, out List<VertexReference> outVertices)
	{
		string[] lines = File.ReadAllLines(path);

		List<Vector3> positions = new();
		List<Vector2> texCoords = new();
		List<Vector3> normals = new();
		List<VertexReference> vertices = new();

		for (int i = 0; i < lines.Length; i++)
		{
			int lineNumber = i + 1;

			string line = lines[i];
			string[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (values.Length == 0)
				continue;

			string[] coords = values[1..];
			switch (values[0])
			{
				case "v":
					if (coords.Length < 3)
						throw new($"Invalid position (v) on line {lineNumber}. Must contain at least 3 coordinates. (Additional coordinates are ignored.)");

					positions.Add(new(ParseVertexValue(coords[0]), ParseVertexValue(coords[1]), ParseVertexValue(coords[2])));
					break;
				case "vt":
					if (coords.Length < 2)
						throw new($"Invalid texture (vt) on line {lineNumber}. Must contain at least 2 coordinates. (Additional coordinates are ignored.)");

					texCoords.Add(new(ParseVertexValue(coords[0]), ParseVertexValue(coords[1])));
					break;
				case "vn":
					if (coords.Length < 3)
						throw new($"Invalid normal (vn) on line {lineNumber}. Must contain at least 3 coordinates. (Additional coordinates are ignored.)");

					normals.Add(new(ParseVertexValue(coords[0]), ParseVertexValue(coords[1]), ParseVertexValue(coords[2])));
					break;
				case "f":
					// Compatible with:
					// f 1 2 3
					// f 1/2/3 4/5/6 7/8/9
					if (coords.Length < 3)
						throw new NotSupportedException($"Invalid face on line {lineNumber}. Must be a complete triangle.");

					if (coords.Length > 3)
						throw new NotSupportedException($"Invalid face on line {lineNumber}. Quads and NGons are not supported. Export your meshes as triangles.");

					for (int j = 0; j < 3; j++)
					{
						string value = coords[j];

						string baseErrorMessage = $"Invalid vertex data in file '{Path.GetFileName(path)}' on line {lineNumber}:";

						if (value.Contains('/'))
						{
							// f 1/2/3 4/5/6 7/8/9
							string[] references = value.Split('/');

							if (references.Length != 3)
								throw new($"Invalid face data on line {lineNumber}. Must contain reference to position, texture (UV), and normal coordinates.");

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

							vertices.Add(new(positionReference, texCoordReference, normalReference));
						}
						else
						{
							// f 1 2 3
							if (string.IsNullOrWhiteSpace(value))
								throw new($"{baseErrorMessage} No vertex value found. This probably means your model file is corrupted.");
							if (!uint.TryParse(value, out uint unifiedValue))
								throw new($"{baseErrorMessage} Value '{value}' could not be parsed to a positive integral value ({typeof(uint).Name}).");

							vertices.Add(new(unifiedValue));
						}
					}

					break;
			}
		}

		outPositions = new();
		outTexCoords = new();
		outNormals = new();
		outVertices = new();

		// Duplicate vertices as needed.
		for (uint i = 0; i < vertices.Count; i += 3)
		{
			// Three vertices make up one face.
			VertexReference vertex1 = vertices[(int)i];
			VertexReference vertex2 = vertices[(int)i + 1];
			VertexReference vertex3 = vertices[(int)i + 2];

			if (positions.Count < vertex1.PositionReference)
				throw new($"Face vertex 1 targets position {vertex1.PositionReference} but there are only {positions.Count} positions.");
			if (positions.Count < vertex2.PositionReference)
				throw new($"Face vertex 2 targets position {vertex2.PositionReference} but there are only {positions.Count} positions.");
			if (positions.Count < vertex3.PositionReference)
				throw new($"Face vertex 3 targets position {vertex3.PositionReference} but there are only {positions.Count} positions.");

			outPositions.Add(positions[(int)vertex1.PositionReference - 1]);
			outPositions.Add(positions[(int)vertex2.PositionReference - 1]);
			outPositions.Add(positions[(int)vertex3.PositionReference - 1]);

			if (texCoords.Count < vertex1.TexCoordReference)
				throw new($"Face vertex 1 targets position {vertex1.TexCoordReference} but there are only {texCoords.Count} texture coordinates.");
			if (texCoords.Count < vertex2.TexCoordReference)
				throw new($"Face vertex 2 targets position {vertex2.TexCoordReference} but there are only {texCoords.Count} texture coordinates.");
			if (texCoords.Count < vertex3.TexCoordReference)
				throw new($"Face vertex 3 targets position {vertex3.TexCoordReference} but there are only {texCoords.Count} texture coordinates.");

			outTexCoords.Add(texCoords[(int)vertex1.TexCoordReference - 1]);
			outTexCoords.Add(texCoords[(int)vertex2.TexCoordReference - 1]);
			outTexCoords.Add(texCoords[(int)vertex3.TexCoordReference - 1]);

			if (normals.Count < vertex1.NormalReference)
				throw new($"Face vertex 1 targets position {vertex1.NormalReference} but there are only {normals.Count} normals.");
			if (normals.Count < vertex2.NormalReference)
				throw new($"Face vertex 2 targets position {vertex2.NormalReference} but there are only {normals.Count} normals.");
			if (normals.Count < vertex3.NormalReference)
				throw new($"Face vertex 3 targets position {vertex3.NormalReference} but there are only {normals.Count} normals.");

			outNormals.Add(normals[(int)vertex1.NormalReference - 1]);
			outNormals.Add(normals[(int)vertex2.NormalReference - 1]);
			outNormals.Add(normals[(int)vertex3.NormalReference - 1]);

			outVertices.Add(new(i + 1));
			outVertices.Add(new(i + 2));
			outVertices.Add(new(i + 3));
		}
	}

	public override IEnumerable<FileResult> ExtractBinary()
	{
		uint indexCount = BitConverter.ToUInt32(Buffer, 0);
		uint vertexCount = BitConverter.ToUInt32(Buffer, 4);

		Vertex[] vertices = new Vertex[vertexCount];
		uint[] indices = new uint[indexCount];

		for (int i = 0; i < vertices.Length; i++)
			vertices[i] = Vertex.CreateFromBuffer(Buffer, HeaderSize, i);

		for (int i = 0; i < indices.Length; i++)
			indices[i] = BitConverter.ToUInt32(Buffer, HeaderSize + vertices.Length * Vertex.ByteCount + i * sizeof(uint));

		StringBuilder sb = new();
		sb.Append("# ").Append(Name).AppendLine(".obj\n");

		sb.AppendLine("# Vertex Attributes");
		StringBuilder v = new();
		StringBuilder vt = new();
		StringBuilder vn = new();
		for (uint i = 0; i < vertexCount; ++i)
		{
			v.Append("v ").Append(vertices[i].Position.X).Append(' ').Append(vertices[i].Position.Y).Append(' ').Append(vertices[i].Position.Z).AppendLine();
			vt.Append("vt ").Append(vertices[i].TexCoord.X).Append(' ').Append(vertices[i].TexCoord.Y).AppendLine();
			vn.Append("vn ").Append(vertices[i].Normal.X).Append(' ').Append(vertices[i].Normal.Y).Append(' ').Append(vertices[i].Normal.Z).AppendLine();
		}

		sb.Append(v);
		sb.Append(vt);
		sb.Append(vn);

		sb.AppendLine("\n# Triangles");
		for (uint i = 0; i < indexCount / 3; ++i)
		{
			VertexReference vertex1 = new(indices[i * 3] + 1);
			VertexReference vertex2 = new(indices[i * 3 + 1] + 1);
			VertexReference vertex3 = new(indices[i * 3 + 2] + 1);
			sb.Append("f ").Append(vertex1).Append(' ').Append(vertex2).Append(' ').Append(vertex3).AppendLine();
		}

		yield return new(Name, Encoding.Default.GetBytes(sb.ToString()));
	}

	public override bool IsBinaryEqual(Chunk? otherChunk, out string? diffReason)
	{
		if (otherChunk == null)
		{
			diffReason = "Other chunk is not present.";
			return false;
		}

		uint indexCount = BitConverter.ToUInt32(Buffer, 0);
		uint otherIndexCount = BitConverter.ToUInt32(otherChunk.Buffer, 0);
		if (indexCount != otherIndexCount)
		{
			diffReason = $"Index counts are not equal ({indexCount} - {otherIndexCount}).";
			return false;
		}

		uint vertexCount = BitConverter.ToUInt32(Buffer, 0);
		uint otherVertexCount = BitConverter.ToUInt32(otherChunk.Buffer, 0);
		if (vertexCount != otherVertexCount)
		{
			diffReason = $"Vertex counts are not equal ({vertexCount} - {otherVertexCount}).";
			return false;
		}

		for (int i = 0; i < vertexCount; i++)
		{
			Vertex vertex = Vertex.CreateFromBuffer(Buffer, HeaderSize, i);
			Vertex otherVertex = Vertex.CreateFromBuffer(otherChunk.Buffer, HeaderSize, i);

			vertex.RoundValues(3);
			otherVertex.RoundValues(3);

			if (vertex.Position != otherVertex.Position)
			{
				diffReason = $"Vertex positions {i} are not equal ({vertex.Position} - {otherVertex.Position}).";
				return false;
			}

			if (vertex.TexCoord != otherVertex.TexCoord)
			{
				diffReason = $"Vertex texture coordinates {i} are not equal ({vertex.TexCoord} - {otherVertex.TexCoord}).";
				return false;
			}

			if (vertex.Normal != otherVertex.Normal)
			{
				diffReason = $"Vertex normals {i} are not equal ({vertex.Normal} - {otherVertex.Normal}).";
				return false;
			}
		}

		for (int i = 0; i < indexCount; i++)
		{
			uint index = BitConverter.ToUInt32(Buffer, HeaderSize + (int)vertexCount * Vertex.ByteCount + i * sizeof(uint));
			uint otherIndex = BitConverter.ToUInt32(otherChunk.Buffer, HeaderSize + (int)vertexCount * Vertex.ByteCount + i * sizeof(uint));
			if (index != otherIndex)
			{
				diffReason = $"Indices {i} are not equal ({index} - {otherIndex}).";
				return false;
			}
		}

		diffReason = null;
		return true;
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
			Vector3 position = new(
				x: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount),
				y: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 4),
				z: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 8));
			Vector2 texCoord = new(
				x: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 24),
				y: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 28));
			Vector3 normal = new(
				x: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 12),
				y: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 16),
				z: BitConverter.ToSingle(buffer, offset + vertexIndex * ByteCount + 20));
			return new(position, texCoord, normal);
		}

		public void RoundValues(int decimals)
		{
			Position = new Vector3(
				(float)Math.Round((decimal)Position.X, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round((decimal)Position.Y, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round((decimal)Position.Z, decimals, MidpointRounding.AwayFromZero));

			TexCoord = new Vector2(
				(float)Math.Round((decimal)TexCoord.X, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round((decimal)TexCoord.Y, decimals, MidpointRounding.AwayFromZero));

			Normal = new Vector3(
				(float)Math.Round((decimal)Normal.X, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round((decimal)Normal.Y, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round((decimal)Normal.Z, decimals, MidpointRounding.AwayFromZero));
		}
	}
}
