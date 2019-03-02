using DevilDaggersAssetCore.Headers;

namespace DevilDaggersAssetCore.Chunks
{
	public class TextureChunk : AbstractHeaderedChunk<TextureHeader>
	{
		public override string FileExtension => ".png";
		public override string FolderName => "Textures";

		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		//public override bool TryExtract(out byte[] result)
		//{
		//	uint width = Header.Width >> file;
		//	uint height = Header.Height >> file;

		//	uint offset = 0;
		//	while (file > 0)
		//	{
		//		offset += (m_header->m_width >> (file - 1)) * (m_header->m_height >> (file - 1)) * 4;
		//		--file;
		//	}

		//	unsigned char* buffer = reinterpret_cast < unsigned char*> (m_buffer) + offset;

		//	uint32_t rowLength = width * 4;
		//	unsigned char* flippedBuffer = new unsigned char[rowLength * height];
		//	for (uint32_t y = 0; y < height; ++y)
		//	{
		//		memcpy(flippedBuffer + y * rowLength, buffer + (height - y - 1) * rowLength, rowLength);
		//	}
		//	buffer = flippedBuffer;
		//	delete[] flippedBuffer;

		//	return true;
		//}
	}
}