using System;

namespace DevilDaggersAssetCore
{
	[Flags]
	public enum BinaryFileType
	{
		Audio = 1,
		DD = 2,
		Core = 4,
		Particle = 8
	}
}