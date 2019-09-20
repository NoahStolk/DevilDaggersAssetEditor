﻿using System;

namespace DevilDaggersAssetCore
{
	public static class Extensions
	{
		public static string GetSubfolderName(this BinaryFileType binaryFileType)
		{
			switch (binaryFileType)
			{
				case BinaryFileType.Audio:
				case BinaryFileType.DD:
					return "res";
				case BinaryFileType.Core:
					return "core";
				case BinaryFileType.Particle:
					return "dd";
				default: throw new Exception($"{nameof(BinaryFileType)} '{binaryFileType}' has not been implemented in this method.");
			}
		}

		public static bool HasFlagBothWays(this BinaryFileType a, BinaryFileType b)
		{
			return a.HasFlag(b) || b.HasFlag(a);
		}
	}
}