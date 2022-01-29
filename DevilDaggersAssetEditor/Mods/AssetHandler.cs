using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DevilDaggersAssetEditor.Mods;

public sealed class AssetHandler
{
	private static readonly Lazy<AssetHandler> _lazy = new(() => new());

	private AssetHandler()
	{
		using StreamReader srAudioAudio = new(GetContentStream("audio.Audio.json"));
		AudioAudioAssets = JsonConvert.DeserializeObject<List<AudioAssetData>>(srAudioAudio.ReadToEnd()) ?? throw new("Corrupt audio.Audio.json.");
		AudioAudioAssets.ForEach(a => a.AssetType = AssetType.Audio);

		using StreamReader srCoreShaders = new(GetContentStream("core.Shaders.json"));
		CoreShadersAssets = JsonConvert.DeserializeObject<List<AssetData>>(srCoreShaders.ReadToEnd()) ?? throw new("Corrupt core.Shaders.json.");
		CoreShadersAssets.ForEach(a => a.AssetType = AssetType.Shader);

		using StreamReader srDdModelBindings = new(GetContentStream("dd.Model Bindings.json"));
		DdModelBindingsAssets = JsonConvert.DeserializeObject<List<AssetData>>(srDdModelBindings.ReadToEnd()) ?? throw new("Corrupt dd.Model Bindings.json.");
		DdModelBindingsAssets.ForEach(a => a.AssetType = AssetType.ModelBinding);

		using StreamReader srDdModels = new(GetContentStream("dd.Models.json"));
		DdModelsAssets = JsonConvert.DeserializeObject<List<ModelAssetData>>(srDdModels.ReadToEnd()) ?? throw new("Corrupt dd.Models.json.");
		DdModelsAssets.ForEach(a => a.AssetType = AssetType.Model);

		using StreamReader srDdShaders = new(GetContentStream("dd.Shaders.json"));
		DdShadersAssets = JsonConvert.DeserializeObject<List<AssetData>>(srDdShaders.ReadToEnd()) ?? throw new("Corrupt dd.Shaders.json.");
		DdShadersAssets.ForEach(a => a.AssetType = AssetType.Shader);

		using StreamReader srDdTextures = new(GetContentStream("dd.Textures.json"));
		DdTexturesAssets = JsonConvert.DeserializeObject<List<TextureAssetData>>(srDdTextures.ReadToEnd()) ?? throw new("Corrupt dd.Textures.json.");
		DdTexturesAssets.ForEach(a => a.AssetType = AssetType.Texture);

		static Stream GetContentStream(string relativeContentName)
			=> Assembly.GetExecutingAssembly().GetManifestResourceStream($"DevilDaggersCore.Content.{relativeContentName}") ?? throw new($"Could not retrieve content stream '{relativeContentName}'.");
	}

	public static AssetHandler Instance => _lazy.Value;

	public List<AudioAssetData> AudioAudioAssets { get; }
	public List<AssetData> CoreShadersAssets { get; }
	public List<AssetData> DdModelBindingsAssets { get; }
	public List<ModelAssetData> DdModelsAssets { get; }
	public List<AssetData> DdShadersAssets { get; }
	public List<TextureAssetData> DdTexturesAssets { get; }
}
