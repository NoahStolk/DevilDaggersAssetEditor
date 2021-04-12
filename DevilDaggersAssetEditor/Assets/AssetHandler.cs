using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Assets
{
	public sealed class AssetHandler
	{
		private static readonly Lazy<AssetHandler> _lazy = new(() => new());

		private AssetHandler()
		{
			using StreamReader srAudioAudio = new(AssemblyUtils.GetContentStream("audio.Audio.json"));
			AudioAudioAssets = JsonConvert.DeserializeObject<List<AudioAsset>>(srAudioAudio.ReadToEnd()) ?? throw new("Corrupt audio.Audio.json.");

			using StreamReader srCoreShaders = new(AssemblyUtils.GetContentStream("core.Shaders.json"));
			CoreShadersAssets = JsonConvert.DeserializeObject<List<ShaderAsset>>(srCoreShaders.ReadToEnd()) ?? throw new("Corrupt core.Shaders.json.");

			using StreamReader srDdModelBindings = new(AssemblyUtils.GetContentStream("dd.Model Bindings.json"));
			DdModelBindingsAssets = JsonConvert.DeserializeObject<List<ModelBindingAsset>>(srDdModelBindings.ReadToEnd()) ?? throw new("Corrupt dd.Model Bindings.json.");

			using StreamReader srDdModels = new(AssemblyUtils.GetContentStream("dd.Models.json"));
			DdModelsAssets = JsonConvert.DeserializeObject<List<ModelAsset>>(srDdModels.ReadToEnd()) ?? throw new("Corrupt dd.Models.json.");

			using StreamReader srDdShaders = new(AssemblyUtils.GetContentStream("dd.Shaders.json"));
			DdShadersAssets = JsonConvert.DeserializeObject<List<ShaderAsset>>(srDdShaders.ReadToEnd()) ?? throw new("Corrupt dd.Shaders.json.");

			using StreamReader srDdTextures = new(AssemblyUtils.GetContentStream("dd.Textures.json"));
			DdTexturesAssets = JsonConvert.DeserializeObject<List<TextureAsset>>(srDdTextures.ReadToEnd()) ?? throw new("Corrupt dd.Textures.json.");
		}

		public static AssetHandler Instance => _lazy.Value;

		public List<AudioAsset> AudioAudioAssets { get; }
		public List<ShaderAsset> CoreShadersAssets { get; }
		public List<ModelBindingAsset> DdModelBindingsAssets { get; }
		public List<ModelAsset> DdModelsAssets { get; }
		public List<ShaderAsset> DdShadersAssets { get; }
		public List<TextureAsset> DdTexturesAssets { get; }

		public List<AbstractAsset> GetAssets(BinaryFileType binaryFileType, string assetType)
		{
			string id = $"{binaryFileType.ToString().ToLower()}.{assetType.ToLower()}";

			return id switch
			{
				"audio.audio" => AudioAudioAssets.Cast<AbstractAsset>().ToList(),
				"core.shaders" => CoreShadersAssets.Cast<AbstractAsset>().ToList(),
				"dd.model bindings" => DdModelBindingsAssets.Cast<AbstractAsset>().ToList(),
				"dd.models" => DdModelsAssets.Cast<AbstractAsset>().ToList(),
				"dd.shaders" => DdShadersAssets.Cast<AbstractAsset>().ToList(),
				"dd.textures" => DdTexturesAssets.Cast<AbstractAsset>().ToList(),
				_ => throw new($"No asset data found for binary file type '{binaryFileType}' and asset type '{assetType}.'"),
			};
		}
	}
}
