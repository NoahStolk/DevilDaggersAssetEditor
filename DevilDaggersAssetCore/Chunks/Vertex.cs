using System;

namespace DevilDaggersAssetCore.Chunks
{
	public struct Vertex
	{
		public const int ByteCount = 32;

		public float[] Position { get; set; }
		public float[] UV { get; set; }
		public float[] Normal { get; set; }

		public Vertex(byte[] buffer, int i)
		{
			Position = new float[3]
			{
				BitConverter.ToSingle(buffer, i * ByteCount),
				BitConverter.ToSingle(buffer, i * ByteCount + 4),
				BitConverter.ToSingle(buffer, i * ByteCount + 8)
			};
			UV = new float[2]
			{
				BitConverter.ToSingle(buffer, i * ByteCount + 24),
				BitConverter.ToSingle(buffer, i * ByteCount + 28)
			};
			Normal = new float[3]
			{
				BitConverter.ToSingle(buffer, i * ByteCount + 12),
				BitConverter.ToSingle(buffer, i * ByteCount + 16),
				BitConverter.ToSingle(buffer, i * ByteCount + 20)
			};
		}

		public byte[] ToByteArray()
		{
			byte[] bytes = new byte[ByteCount];
			Buffer.BlockCopy(BitConverter.GetBytes(Position != null ? Position[0] : 0), 0, bytes, 0, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(Position != null ? Position[1] : 0), 0, bytes, 4, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(Position != null ? Position[2] : 0), 0, bytes, 8, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(Normal != null ? Normal[0] : 0), 0, bytes, 12, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(Normal != null ? Normal[1] : 0), 0, bytes, 16, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(Normal != null ? Normal[2] : 0), 0, bytes, 20, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(UV != null ? UV[0] : 0), 0, bytes, 24, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(UV != null ? UV[1] : 0), 0, bytes, 28, sizeof(float));
			return bytes;
		}
	}
}