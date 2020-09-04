using System;

namespace DevilDaggersAssetEditor.Info
{
	public class HeaderInfo
	{
		public Type ChunkHeaderType { get; }
		public uint? FixedSize { get; }

		public HeaderInfo(Type chunkHeaderType, uint? fixedSize)
		{
			ChunkHeaderType = chunkHeaderType;
			FixedSize = fixedSize;
		}
	}
}