using DevilDaggersCore.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Binaries.Chunks;

public class ShaderChunk : Chunk
{
	public ShaderChunk(string name, uint startOffset, uint size)
		: base(AssetType.Shader, name, startOffset, size)
	{
	}

	public override int HeaderSize => 12;

	public override void MakeBinary(string path)
	{
		string vertexPath = path;
		string fragmentPath = path.Replace("_vertex.glsl", "_fragment.glsl");

		byte[] vertexBuffer = File.ReadAllBytes(vertexPath);
		byte[] fragmentBuffer = File.ReadAllBytes(fragmentPath);
		string name = Path.GetFileNameWithoutExtension(path);

		uint nameLength = (uint)name.Length;
		uint vertexSize = (uint)vertexBuffer.Length;
		uint fragmentSize = (uint)fragmentBuffer.Length;

		Buffer = new byte[HeaderSize + nameLength + vertexBuffer.Length + fragmentBuffer.Length];
		Buf.BlockCopy(BitConverter.GetBytes(nameLength), 0, Buffer, 0, sizeof(uint));
		Buf.BlockCopy(BitConverter.GetBytes(vertexSize), 0, Buffer, 4, sizeof(uint));
		Buf.BlockCopy(BitConverter.GetBytes(fragmentSize), 0, Buffer, 8, sizeof(uint));
		Buf.BlockCopy(Encoding.Default.GetBytes(name), 0, Buffer, HeaderSize, (int)nameLength);
		Buf.BlockCopy(vertexBuffer, 0, Buffer, HeaderSize + (int)nameLength, vertexBuffer.Length);
		Buf.BlockCopy(fragmentBuffer, 0, Buffer, HeaderSize + (int)nameLength + vertexBuffer.Length, fragmentBuffer.Length);

		Size = (uint)Buffer.Length;
	}

	public override IEnumerable<FileResult> ExtractBinary()
	{
		uint nameLength = BitConverter.ToUInt32(Buffer, 0);
		uint vertexSize = BitConverter.ToUInt32(Buffer, 4);
		uint fragmentSize = BitConverter.ToUInt32(Buffer, 8);

		byte[] vertexBuffer = new byte[vertexSize];
		Buf.BlockCopy(Buffer, (int)nameLength + HeaderSize, vertexBuffer, 0, (int)vertexSize);
		yield return new($"{Name}_vertex", vertexBuffer);

		byte[] fragmentBuffer = new byte[fragmentSize];
		Buf.BlockCopy(Buffer, (int)nameLength + HeaderSize + (int)vertexSize, fragmentBuffer, 0, (int)fragmentSize);
		yield return new($"{Name}_fragment", fragmentBuffer);
	}

	public override bool IsBinaryEqual(Chunk? otherChunk, out string? diffReason)
	{
		if (otherChunk == null)
		{
			diffReason = "Other chunk is not present.";
			return false;
		}

		FileResult[] originalFiles = ExtractBinary().ToArray();
		FileResult[] otherFiles = otherChunk.ExtractBinary().ToArray();

		for (int i = 0; i < 2; i++)
		{
			FileResult originalFile = originalFiles[i];
			FileResult otherFile = otherFiles[i];

			if (originalFile.Buffer.Length != otherFile.Buffer.Length)
			{
				diffReason = $"Parts '{originalFile.Name}' do not have the same length ({Buffer.Length} - {otherChunk.Buffer.Length}).";
				return false;
			}

			for (int j = 0; j < originalFile.Buffer.Length; j++)
			{
				if (originalFile.Buffer[j] != otherFile.Buffer[j])
				{
					diffReason = $"Bytes at position {j} in part '{originalFile.Name}' do not match (0x{originalFile.Buffer[j]:X} - 0x{otherFile.Buffer[j]:X}).";
					return false;
				}
			}
		}

		diffReason = null;
		return true;
	}
}