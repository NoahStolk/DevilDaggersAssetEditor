using DevilDaggersAssetEditor.BinaryFileHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ShaderChunk : AbstractResourceChunk
	{
		public ShaderChunk(string name, uint startOffset, uint size)
			: base(name, startOffset, size)
		{
		}

		public override void MakeBinary(string path)
		{
			string vertexPath = path.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture);
			string fragmentPath = path.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture);

			byte[] vertexBuffer = File.ReadAllBytes(vertexPath);
			byte[] fragmentBuffer = File.ReadAllBytes(fragmentPath);

			string name = Path.GetFileNameWithoutExtension(path);
			uint nameLength = (uint)name.Length;

			Buffer = new byte[nameLength + vertexBuffer.Length + fragmentBuffer.Length];
			Buf.BlockCopy(Encoding.Default.GetBytes(name), 0, Buffer, 0, (int)nameLength);
			Buf.BlockCopy(vertexBuffer, 0, Buffer, (int)nameLength, vertexBuffer.Length);
			Buf.BlockCopy(fragmentBuffer, 0, Buffer, (int)nameLength + vertexBuffer.Length, fragmentBuffer.Length);

			uint vertexSize = (uint)vertexBuffer.Length;
			uint fragmentSize = (uint)fragmentBuffer.Length;

			byte[] headerBuffer = new byte[12];
			Buf.BlockCopy(BitConverter.GetBytes(nameLength), 0, headerBuffer, 0, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(vertexSize), 0, headerBuffer, 4, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(fragmentSize), 0, headerBuffer, 8, sizeof(uint));

			Size = (uint)Buffer.Length + (uint)headerBuffer.Length;
		}

		public override IEnumerable<FileResult> ExtractBinary()
		{
			uint nameLength = BitConverter.ToUInt32(Buffer, 0);
			uint vertexSize = BitConverter.ToUInt32(Buffer, 4);
			uint fragmentSize = BitConverter.ToUInt32(Buffer, 8);

			byte[] vertexBuffer = new byte[vertexSize];
			Buf.BlockCopy(Buffer, (int)nameLength + 12, vertexBuffer, 0, (int)vertexSize);
			yield return new FileResult($"{Name}_vertex", vertexBuffer);

			byte[] fragmentBuffer = new byte[fragmentSize];
			Buf.BlockCopy(Buffer, (int)nameLength + 12 + (int)vertexSize, fragmentBuffer, 0, (int)fragmentSize);
			yield return new FileResult($"{Name}_fragment", fragmentBuffer);
		}
	}
}