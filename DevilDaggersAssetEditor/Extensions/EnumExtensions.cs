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
				_ => throw new NotSupportedException($"{nameof(BinaryFileType)} '{binaryFileType}' is not supported in the {nameof(GetSubfolderName)} method."),
			};
		}

		public static string GetFileExtension(this AssetType assetType)
		{
			return assetType switch
			{
				AssetType.Audio => ".wav",
				AssetType.ModelBinding => ".txt",
				AssetType.Model => ".obj",
				AssetType.Shader => ".glsl",
				AssetType.Texture => ".png",
				_ => throw new NotSupportedException($"{nameof(AssetType)} '{assetType}' is not supported in the {nameof(GetFileExtension)} method."),
			};
		}

		public static string GetFolderName(this AssetType assetType)
		{
			return assetType switch
			{
				AssetType.Audio => "Audio",
				AssetType.ModelBinding => "Model Bindings",
				AssetType.Model => "Models",
				AssetType.Shader => "Shaders",
				AssetType.Texture => "Textures",
				_ => throw new NotSupportedException($"{nameof(AssetType)} '{assetType}' is not supported in the {nameof(GetFolderName)} method."),
			};
		}

		public static (byte R, byte G, byte B) GetColor(this AssetType assetType)
		{
			return assetType switch
			{
				AssetType.Audio => (255, 0, 255),
				AssetType.ModelBinding => (0, 255, 255),
				AssetType.Model => (255, 0, 0),
				AssetType.Shader => (0, 255, 0),
				AssetType.Texture => (255, 127, 0),
				_ => throw new NotSupportedException($"{nameof(AssetType)} '{assetType}' is not supported in the {nameof(GetFolderName)} method."),
			};
		}

		public static byte GetBinaryType(this AssetType assetType)
		{
			return assetType switch
			{
				AssetType.Model => 0x01,
				AssetType.Texture => 0x02,
				AssetType.Shader => 0x10,
				AssetType.Audio => 0x20,
				AssetType.ModelBinding => 0x80,
				_ => throw new NotSupportedException($"{nameof(AssetType)} '{assetType}' is not supported in the {nameof(GetBinaryType)} method."),
			};
		}

		public static AssetType? GetAssetType(this string fileExtension)
		{
			return fileExtension switch
			{
				".wav" => AssetType.Audio,
				".ini" => AssetType.Audio,
				".txt" => AssetType.ModelBinding,
				".obj" => AssetType.Model,
				".glsl" => AssetType.Shader,
				".png" => AssetType.Texture,
				_ => null,
			};
		}

		public static AssetType? GetAssetType(this byte binaryType)
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
