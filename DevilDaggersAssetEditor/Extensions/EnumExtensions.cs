﻿using DevilDaggersAssetEditor.BinaryFileHandlers;
using System;

namespace DevilDaggersAssetEditor.Extensions
{
	public static class EnumExtensions
	{
		public static string GetSubfolderName(this BinaryFileType binaryFileType)
		{
			return binaryFileType switch
			{
				BinaryFileType.Audio => "res",
				BinaryFileType.Dd => "res",
				BinaryFileType.Core => "core",
				BinaryFileType.Particle => "dd",
				_ => throw new NotImplementedException($"{nameof(BinaryFileType)} '{binaryFileType}' has not been implemented in the {nameof(GetSubfolderName)} method."),
			};
		}
	}
}