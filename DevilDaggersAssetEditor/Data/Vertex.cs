using System;
using System.Numerics;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Data
{
	public struct Vertex
	{
		public const int ByteCount = 32;

		public Vector3 Position { get; set; }
		public Vector2 TexCoord { get; set; }
		public Vector3 Normal { get; set; }

		public Vertex(Vector3 position, Vector2 texCoord, Vector3 normal)
		{
			Position = position;
			TexCoord = texCoord;
			Normal = normal;
		}

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

		public static Vertex CreateFromBuffer(byte[] buffer, int vertexIndex)
		{
			Vector3 position = new Vector3(
				x: BitConverter.ToSingle(buffer, vertexIndex * ByteCount),
				y: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 4),
				z: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 8));
			Vector2 texCoord = new Vector2(
				x: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 24),
				y: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 28));
			Vector3 normal = new Vector3(
				x: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 12),
				y: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 16),
				z: BitConverter.ToSingle(buffer, vertexIndex * ByteCount + 20));
			return new Vertex(position, texCoord, normal);
		}
	}
}