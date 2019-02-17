﻿namespace DevilDaggersAssetCore.Chunks
{
	public class AudioChunk : AbstractChunk
	{
		public override string FileExtension => ".wav";

		public AudioChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}
	}
}