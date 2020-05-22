using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Chunks;
using DevilDaggersAssetCore.Headers;
using DevilDaggersAssetCore.ModFiles;
using System;

namespace DevilDaggersAssetCore.Info
{
	public class ChunkInfo
	{
		public static ChunkInfo Model = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelChunk),
			headerInfo: new HeaderInfo(typeof(ModelHeader), 10),
			assetType: typeof(ModelAsset),
			userAssetType: typeof(ModelUserAsset),
			binaryTypes: new ushort[] { 0x01 },
			fileExtension: ".obj",
			folderName: "Models",
			dataName: "Model",
			colorR: 255,
			colorG: 0,
			colorB: 0);

		public static ChunkInfo Texture = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(TextureChunk),
			headerInfo: new HeaderInfo(typeof(TextureHeader), 11),
			assetType: typeof(TextureAsset),
			userAssetType: typeof(TextureUserAsset),
			binaryTypes: new ushort[] { 0x02 },
			fileExtension: ".png",
			folderName: "Textures",
			dataName: "Texture",
			colorR: 255,
			colorG: 127,
			colorB: 0);

		public static ChunkInfo Shader = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd | BinaryFileType.Core,
			chunkType: typeof(ShaderChunk),
			headerInfo: new HeaderInfo(typeof(ShaderHeader), 12),
			assetType: typeof(ShaderAsset),
			userAssetType: typeof(ShaderUserAsset),
			binaryTypes: new ushort[] { 0x10, 0x11 },
			fileExtension: ".glsl",
			folderName: "Shaders",
			dataName: "Shader",
			colorR: 0,
			colorG: 255,
			colorB: 0);

		public static ChunkInfo Audio = new ChunkInfo(
			binaryFileType: BinaryFileType.Audio,
			chunkType: typeof(AudioChunk),
			headerInfo: null,
			assetType: typeof(AudioAsset),
			userAssetType: typeof(AudioUserAsset),
			binaryTypes: new ushort[] { 0x20 },
			fileExtension: ".wav",
			folderName: "Audio",
			dataName: "Audio",
			colorR: 255,
			colorG: 0,
			colorB: 255);

		public static ChunkInfo ModelBinding = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelBindingChunk),
			headerInfo: null,
			assetType: typeof(ModelBindingAsset),
			userAssetType: typeof(ModelBindingUserAsset),
			binaryTypes: new ushort[] { 0x80 },
			fileExtension: ".txt",
			folderName: "Model Bindings",
			dataName: "Model binding",
			colorR: 0,
			colorG: 255,
			colorB: 255);

		public static ChunkInfo Particle = new ChunkInfo(
			binaryFileType: BinaryFileType.Particle,
			chunkType: typeof(ParticleChunk),
			headerInfo: new HeaderInfo(typeof(ParticleHeader), null),
			assetType: typeof(ParticleAsset),
			userAssetType: typeof(ParticleUserAsset),
			binaryTypes: null,
			fileExtension: ".bin",
			folderName: "Particles",
			dataName: "Particle",
			colorR: 255,
			colorG: 255,
			colorB: 0);

		public static ChunkInfo[] All = new[] { Model, Texture, Shader, Audio, ModelBinding, Particle };

		public BinaryFileType BinaryFileType { get; }
		public Type ChunkType { get; }
		public HeaderInfo HeaderInfo { get; }
		public Type AssetType { get; }
		public Type UserAssetType { get; }
		public ushort[] BinaryTypes { get; }
		public string FileExtension { get; }
		public string FolderName { get; }
		public string DataName { get; }
		public byte ColorR { get; }
		public byte ColorG { get; }
		public byte ColorB { get; }

		public ChunkInfo(BinaryFileType binaryFileType, Type chunkType, HeaderInfo headerInfo, Type assetType, Type userAssetType, ushort[] binaryTypes, string fileExtension, string folderName, string dataName, byte colorR, byte colorG, byte colorB)
		{
			BinaryFileType = binaryFileType;
			ChunkType = chunkType;
			HeaderInfo = headerInfo;
			AssetType = assetType;
			UserAssetType = userAssetType;
			BinaryTypes = binaryTypes;
			FileExtension = fileExtension;
			FolderName = folderName;
			DataName = dataName;
			ColorR = colorR;
			ColorG = colorG;
			ColorB = colorB;
		}
	}
}