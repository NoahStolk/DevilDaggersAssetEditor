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

		public static AssetType GetAssetTypeFromFileExtension(this string fileExtension)
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
				_ => throw new NotSupportedException($"File extension '{fileExtension}' is not supported in the {nameof(GetAssetTypeFromFileExtension)} method."),
			};
		}
	}
}