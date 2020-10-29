using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using System;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Chunks
{
	public class ChunkInfo
	{
		public ChunkInfo(BinaryFileType binaryFileType, Type chunkType, AssetType assetType, string folderName)
		{
			BinaryFileType = binaryFileType;
			ChunkType = chunkType;
			AssetType = assetType;
			FolderName = folderName;
		}

		public static ChunkInfo Model { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelChunk),
			assetType: AssetType.Model,
			folderName: "Models");

		public static ChunkInfo Texture { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(TextureChunk),
			assetType: AssetType.Texture,
			folderName: "Textures");

		public static ChunkInfo Shader { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd | BinaryFileType.Core,
			chunkType: typeof(ShaderChunk),
			assetType: AssetType.Shader,
			folderName: "Shaders");

		public static ChunkInfo Audio { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Audio,
			chunkType: typeof(AudioChunk),
			assetType: AssetType.Audio,
			folderName: "Audio");

		public static ChunkInfo ModelBinding { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Dd,
			chunkType: typeof(ModelBindingChunk),
			assetType: AssetType.ModelBinding,
			folderName: "Model Bindings");

		public static ChunkInfo Particle { get; } = new ChunkInfo(
			binaryFileType: BinaryFileType.Particle,
			chunkType: typeof(ParticleChunk),
			assetType: AssetType.Particle,
			folderName: "Particles");

		public static IReadOnlyList<ChunkInfo> All { get; } = new List<ChunkInfo> { Model, Texture, Shader, Audio, ModelBinding, Particle };

		public BinaryFileType BinaryFileType { get; }
		public Type ChunkType { get; }
		public AssetType AssetType { get; }
		public string FolderName { get; }
	}
}