using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Headers;
using DevilDaggersAssetEditor.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	/// <summary>
	/// Internal structure of a shader chunk:
	/// - <see cref="ShaderHeader">Header</see> (to specify chunk name length, vertex buffer size, fragment buffer size).
	/// - Chunk name.
	/// - Vertex buffer.
	/// - Fragment buffer.
	/// </summary>
	public class ShaderChunk : AbstractHeaderedChunk<ShaderHeader>
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

			byte[] headerBuffer = new byte[ChunkInfo.Shader.HeaderInfo.FixedSize.Value];
			Buf.BlockCopy(BitConverter.GetBytes(nameLength), 0, headerBuffer, 0, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(vertexSize), 0, headerBuffer, 4, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(fragmentSize), 0, headerBuffer, 8, sizeof(uint));
			Header = new ShaderHeader(headerBuffer);

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;
		}

		public override IEnumerable<FileResult> ExtractBinary()
		{
			byte[] vertexBuffer = new byte[Header.VertexSize];
			Buf.BlockCopy(Buffer, (int)Header.NameLength, vertexBuffer, 0, (int)Header.VertexSize);
			yield return new FileResult($"{Name}_vertex", vertexBuffer);

			byte[] fragmentBuffer = new byte[Header.FragmentSize];
			Buf.BlockCopy(Buffer, (int)Header.NameLength + (int)Header.VertexSize, fragmentBuffer, 0, (int)Header.FragmentSize);
			yield return new FileResult($"{Name}_fragment", fragmentBuffer);
		}
	}
}