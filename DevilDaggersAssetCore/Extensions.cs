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
				default: throw new Exception($"{nameof(BinaryFileName)} '{binaryFileName}' has not been implemented in this method.");
			}
		}
	}
}