using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using System;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ChunkInfo
	{
		public ChunkInfo(BinaryFileType binaryFileType, Type chunkType, AssetType assetType, string folderName, string dataName, byte colorR, byte colorG, byte colorB)
		{
			BinaryFileType = binaryFileType;
			ChunkType = chunkType;
			AssetType = assetType;
			FolderName = folderName;
			DataName = dataName;
			ColorR = colorR;
			ColorG = colorG;
			ColorB = colorB;
		}

		public static ChunkInfo Model { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelChunk),
			assetType: AssetType.Model,
			folderName: "Models",
			dataName: "Model",
			colorR: 255,
			colorG: 0,
			colorB: 0);

		public static ChunkInfo Texture { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(TextureChunk),
			assetType: AssetType.Texture,
			folderName: "Textures",
			dataName: "Texture",
			colorR: 255,
			colorG: 127,
			colorB: 0);

		public static ChunkInfo Shader { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd | BinaryFileType.Core,
			chunkType: typeof(ShaderChunk),
			assetType: AssetType.Shader,
			folderName: "Shaders",
			dataName: "Shader",
			colorR: 0,
			colorG: 255,
			colorB: 0);

		public static ChunkInfo Audio { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Audio,
			chunkType: typeof(AudioChunk),
			assetType: AssetType.Audio,
			folderName: "Audio",
			dataName: "Audio",
			colorR: 255,
			colorG: 0,
			colorB: 255);

		public static ChunkInfo ModelBinding { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelBindingChunk),
			assetType: AssetType.ModelBinding,
			folderName: "Model Bindings",
			dataName: "Model binding",
			colorR: 0,
			colorG: 255,
			colorB: 255);

		public static ChunkInfo Particle { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Particle,
			chunkType: typeof(ParticleChunk),
			assetType: AssetType.Particle,
			folderName: "Particles",
			dataName: "Particle",
			colorR: 255,
			colorG: 255,
			colorB: 0);

		public static IReadOnlyList<ChunkInfo> All { get; } = new List<ChunkInfo> { Model, Texture, Shader, Audio, ModelBinding, Particle };

		public BinaryFileType BinaryFileType { get; }
		public Type ChunkType { get; }
		public AssetType AssetType { get; }
		public string FolderName { get; }
		public string DataName { get; }
		public byte ColorR { get; }
		public byte ColorG { get; }
		public byte ColorB { get; }
	}
}