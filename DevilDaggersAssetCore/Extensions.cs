using System;

namespace DevilDaggersAssetCore
{
	public static class Extensions
	{
		public static string GetSubfolderName(this BinaryFileName binaryFileName)
		{
			switch (binaryFileName)
			{
				case BinaryFileName.Audio:
				case BinaryFileName.DD:
					return "res";
				case BinaryFileName.Core:
					return "core";
				case BinaryFileName.Particle:
					return "dd";
				default: throw new Exception($"{nameof(BinaryFileName)} '{binaryFileName}' has not been implemented in this method.");
			}
		}

		public static bool HasFlagBothWays(this BinaryFileName a, BinaryFileName b)
		{
			return a.HasFlag(b) || b.HasFlag(a);
		}
	}
}