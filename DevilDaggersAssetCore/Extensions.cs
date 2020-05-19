using System;
using System.IO;

namespace DevilDaggersAssetCore
{
	public static class Extensions
	{
		public static string GetSubfolderName(this BinaryFileType binaryFileType)
		{
			switch (binaryFileType)
			{
				case BinaryFileType.Audio:
				case BinaryFileType.Dd:
					return "res";
				case BinaryFileType.Core:
					return "core";
				case BinaryFileType.Particle:
					return "dd";
				default:
					throw new NotImplementedException($"{nameof(BinaryFileType)} '{binaryFileType}' has not been implemented in the {nameof(GetSubfolderName)} method.");
			}
		}

		public static bool IsPathValid(this string path)
		{
			try
			{
				return Path.IsPathRooted(path);
			}
			catch
			{
				return false;
			}
		}
	}
}