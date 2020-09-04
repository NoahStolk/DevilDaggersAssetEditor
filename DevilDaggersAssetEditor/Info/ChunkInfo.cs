using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Headers;
using DevilDaggersAssetEditor.ModFiles;
using System;
using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.Info
{
	public class ChunkInfo
	{
		public ChunkInfo(BinaryFileType binaryFileType, Type chunkType, HeaderInfo? headerInfo, Type assetType, Type userAssetType, byte binaryType, string fileExtension, string folderName, string dataName, byte colorR, byte colorG, byte colorB)
		{
			BinaryFileType = binaryFileType;
			ChunkType = chunkType;
			HeaderInfo = headerInfo;
			AssetType = assetType;
			UserAssetType = userAssetType;
			BinaryType = binaryType;
			FileExtension = fileExtension;
			FolderName = folderName;
			DataName = dataName;
			ColorR = colorR;
			ColorG = colorG;
			ColorB = colorB;
		}

		public static ChunkInfo Model { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelChunk),
			headerInfo: new HeaderInfo(typeof(ModelHeader), 10),
			assetType: typeof(ModelAsset),
			userAssetType: typeof(ModelUserAsset),
			binaryType: 0x01,
			fileExtension: ".obj",
			folderName: "Models",
			dataName: "Model",
			colorR: 255,
			colorG: 0,
			colorB: 0);

		public static ChunkInfo Texture { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(TextureChunk),
			headerInfo: new HeaderInfo(typeof(TextureHeader), 11),
			assetType: typeof(TextureAsset),
			userAssetType: typeof(TextureUserAsset),
			binaryType: 0x02,
			fileExtension: ".png",
			folderName: "Textures",
			dataName: "Texture",
			colorR: 255,
			colorG: 127,
			colorB: 0);

		public static ChunkInfo Shader { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd | BinaryFileType.Core,
			chunkType: typeof(ShaderChunk),
			headerInfo: new HeaderInfo(typeof(ShaderHeader), 12),
			assetType: typeof(ShaderAsset),
			userAssetType: typeof(ShaderUserAsset),
			binaryType: 0x10,
			fileExtension: ".glsl",
			folderName: "Shaders",
			dataName: "Shader",
			colorR: 0,
			colorG: 255,
			colorB: 0);

		public static ChunkInfo Audio { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Audio,
			chunkType: typeof(AudioChunk),
			headerInfo: null,
			assetType: typeof(AudioAsset),
			userAssetType: typeof(AudioUserAsset),
			binaryType: 0x20,
			fileExtension: ".wav",
			folderName: "Audio",
			dataName: "Audio",
			colorR: 255,
			colorG: 0,
			colorB: 255);

		public static ChunkInfo ModelBinding { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelBindingChunk),
			headerInfo: null,
			assetType: typeof(ModelBindingAsset),
			userAssetType: typeof(ModelBindingUserAsset),
			binaryType: 0x80,
			fileExtension: ".txt",
			folderName: "Model Bindings",
			dataName: "Model binding",
			colorR: 0,
			colorG: 255,
			colorB: 255);

		public static ChunkInfo Particle { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Particle,
			chunkType: typeof(ParticleChunk),
			headerInfo: new HeaderInfo(typeof(ParticleHeader), null),
			assetType: typeof(ParticleAsset),
			userAssetType: typeof(ParticleUserAsset),
			binaryType: 0x00, // Doesn't actually have a binary type.
			fileExtension: ".bin",
			folderName: "Particles",
			dataName: "Particle",
			colorR: 255,
			colorG: 255,
			colorB: 0);

		public static ImmutableArray<ChunkInfo> All { get; } = new ImmutableArray<ChunkInfo> { Model, Texture, Shader, Audio, ModelBinding, Particle };

		public BinaryFileType BinaryFileType { get; }
		public Type ChunkType { get; }
		public HeaderInfo? HeaderInfo { get; }
		public Type AssetType { get; }
		public Type UserAssetType { get; }
		public byte BinaryType { get; }
		public string FileExtension { get; }
		public string FolderName { get; }
		public string DataName { get; }
		public byte ColorR { get; }
		public byte ColorG { get; }
		public byte ColorB { get; }
	}
}