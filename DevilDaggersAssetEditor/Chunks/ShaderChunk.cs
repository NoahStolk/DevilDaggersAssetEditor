using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ShaderChunk : ResourceChunk
	{
		public ShaderChunk(string name, uint startOffset, uint size)
			: base(AssetType.Shader, name, startOffset, size)
		{
		}

		public override void MakeBinary(string path)
		{
			string vertexPath = path;
			string fragmentPath = path.Replace("_vertex.glsl", "_fragment.glsl", StringComparison.InvariantCulture);

			byte[] vertexBuffer = File.ReadAllBytes(vertexPath);
			byte[] fragmentBuffer = File.ReadAllBytes(fragmentPath);
			string name = Path.GetFileNameWithoutExtension(path);

			uint nameLength = (uint)name.Length;
			uint vertexSize = (uint)vertexBuffer.Length;
			uint fragmentSize = (uint)fragmentBuffer.Length;

			Buffer = new byte[12 + nameLength + vertexBuffer.Length + fragmentBuffer.Length];
			Buf.BlockCopy(BitConverter.GetBytes(nameLength), 0, Buffer, 0, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(vertexSize), 0, Buffer, 4, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(fragmentSize), 0, Buffer, 8, sizeof(uint));
			Buf.BlockCopy(Encoding.Default.GetBytes(name), 0, Buffer, 12, (int)nameLength);
			Buf.BlockCopy(vertexBuffer, 0, Buffer, 12 + (int)nameLength, vertexBuffer.Length);
			Buf.BlockCopy(fragmentBuffer, 0, Buffer, 12 + (int)nameLength + vertexBuffer.Length, fragmentBuffer.Length);

			Size = (uint)Buffer.Length;
		}

		public override IEnumerable<FileResult> ExtractBinary()
		{
			uint nameLength = BitConverter.ToUInt32(Buffer, 0);
			uint vertexSize = BitConverter.ToUInt32(Buffer, 4);
			uint fragmentSize = BitConverter.ToUInt32(Buffer, 8);

			byte[] vertexBuffer = new byte[vertexSize];
			Buf.BlockCopy(Buffer, (int)nameLength + 12, vertexBuffer, 0, (int)vertexSize);
			yield return new($"{Name}_vertex", vertexBuffer);

			byte[] fragmentBuffer = new byte[fragmentSize];
			Buf.BlockCopy(Buffer, (int)nameLength + 12 + (int)vertexSize, fragmentBuffer, 0, (int)fragmentSize);
			yield return new($"{Name}_fragment", fragmentBuffer);
		}
	}
}
