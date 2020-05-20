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

		public static PathValidity GetPathValidity(this string path)
		{
			try
			{
				if (Path.IsPathRooted(path) && File.Exists(path))
					return PathValidity.Valid;

				return PathValidity.NotFound;
			}
			catch
			{
				return PathValidity.Invalid;
			}
		}
	}
}