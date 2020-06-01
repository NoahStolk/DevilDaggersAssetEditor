using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetCore.Assets
{
	public sealed class AssetHandler
	{
		public List<AudioAsset> AudioAudioAssets { get; }
		public List<ShaderAsset> CoreShadersAssets { get; }
		public List<ModelBindingAsset> DdModelBindingsAssets { get; }
		public List<ModelAsset> DdModelsAssets { get; }
		public List<ShaderAsset> DdShadersAssets { get; }
		public List<TextureAsset> DdTexturesAssets { get; }
		public List<ParticleAsset> ParticleParticlesAssets { get; }

		private static readonly Lazy<AssetHandler> lazy = new Lazy<AssetHandler>(() => new AssetHandler());
		public static AssetHandler Instance => lazy.Value;

		private AssetHandler()
		{
			using StreamReader srAudioAudio = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.audio.Audio.json"));
			AudioAudioAssets = JsonConvert.DeserializeObject<List<AudioAsset>>(srAudioAudio.ReadToEnd());

			using StreamReader srCoreShaders = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.core.Shaders.json"));
			CoreShadersAssets = JsonConvert.DeserializeObject<List<ShaderAsset>>(srCoreShaders.ReadToEnd());

			using StreamReader srDdModelBindings = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.dd.Model Bindings.json"));
			DdModelBindingsAssets = JsonConvert.DeserializeObject<List<ModelBindingAsset>>(srDdModelBindings.ReadToEnd());

			using StreamReader srDdModels = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.dd.Models.json"));
			DdModelsAssets = JsonConvert.DeserializeObject<List<ModelAsset>>(srDdModels.ReadToEnd());

			using StreamReader srDdShaders = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.dd.Shaders.json"));
			DdShadersAssets = JsonConvert.DeserializeObject<List<ShaderAsset>>(srDdShaders.ReadToEnd());

			using StreamReader srDdTextures = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.dd.Textures.json"));
			DdTexturesAssets = JsonConvert.DeserializeObject<List<TextureAsset>>(srDdTextures.ReadToEnd());

			using StreamReader srParticleParticles = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.particle.Particles.json"));
			ParticleParticlesAssets = JsonConvert.DeserializeObject<List<ParticleAsset>>(srParticleParticles.ReadToEnd());
		}

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
				"particle.particles" => ParticleParticlesAssets.Cast<AbstractAsset>().ToList(),
				_ => throw new Exception($"No asset data found for binary file type '{binaryFileType}' and asset type '{assetType}.'")
			};
		}
	}
}