using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
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
				_ => throw new NotSupportedException($"{nameof(BinaryFileType)} '{binaryFileType}' is not supported in the {nameof(GetSubfolderName)} method."),
			};
		}

		public static string GetFileExtensionFromAssetType(this AssetType assetType)
		{
			return assetType switch
			{
				AssetType.Audio => ".wav",
				AssetType.ModelBinding => ".txt",
				AssetType.Model => ".obj",
				AssetType.Shader => ".glsl",
				AssetType.Texture => ".png",
				AssetType.Particle => ".bin",
				_ => throw new NotSupportedException($"{nameof(AssetType)} '{assetType}' is not supported in the {nameof(GetFileExtensionFromAssetType)} method."),
			};
		}

		public static byte GetBinaryTypeFromAssetType(this AssetType assetType)
		{
			return assetType switch
			{
				AssetType.Model => 0x01,
				AssetType.Texture => 0x02,
				AssetType.Shader => 0x10,
				AssetType.Audio => 0x20,
				AssetType.ModelBinding => 0x80,
				_ => throw new NotSupportedException($"{nameof(AssetType)} '{assetType}' is not supported in the {nameof(GetBinaryTypeFromAssetType)} method."),
			};
		}

		public static AssetType? GetAssetTypeFromFileExtension(this string fileExtension)
		{
			return fileExtension switch
			{
				".wav" => AssetType.Audio,
				".ini" => AssetType.Audio,
				".txt" => AssetType.ModelBinding,
				".obj" => AssetType.Model,
				".glsl" => AssetType.Shader,
				".png" => AssetType.Texture,
				".bin" => AssetType.Particle,
				_ => null,
			};
		}

		public static AssetType? GetAssetTypeFromBinaryType(this byte binaryType)
		{
			return binaryType switch
			{
				0x01 => AssetType.Model,
				0x02 => AssetType.Texture,
				0x10 => AssetType.Shader,
				0x20 => AssetType.Audio,
				0x80 => AssetType.ModelBinding,
				_ => null,
			};
		}
	}
}