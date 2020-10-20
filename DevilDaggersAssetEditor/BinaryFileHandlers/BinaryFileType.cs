using System;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	[Flags]
	public enum BinaryFileType
	{
		None = 0,
		Audio = 1,
		Dd = 2,
		Core = 4,
		Particle = 8,
	}
}