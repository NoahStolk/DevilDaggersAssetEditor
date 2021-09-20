using DevilDaggersAssetEditor.Binaries;
using DevilDaggersCore.Mods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersAssetEditor.Assets
{
	public sealed class AssetContainer
	{
		private static readonly Lazy<AssetContainer> _lazy = new(() => new());

		private AssetContainer()
		{
			AudioAudioAssets = AssetHandler.Instance.AudioAudioAssets.ConvertAll(a => new AudioAsset(a.AssetName, a.IsProhibited, a.DefaultLoudness, a.PresentInDefaultLoudness));
			CoreShadersAssets = AssetHandler.Instance.CoreShadersAssets.ConvertAll(a => new ShaderAsset(a.AssetName, a.IsProhibited));
			DdModelBindingsAssets = AssetHandler.Instance.DdModelBindingsAssets.ConvertAll(a => new ModelBindingAsset(a.AssetName, a.IsProhibited));
			DdModelsAssets = AssetHandler.Instance.DdModelsAssets.ConvertAll(a => new ModelAsset(a.AssetName, a.IsProhibited, a.DefaultIndexCount, a.DefaultVertexCount));
			DdShadersAssets = AssetHandler.Instance.DdShadersAssets.ConvertAll(a => new ShaderAsset(a.AssetName, a.IsProhibited));
			DdTexturesAssets = AssetHandler.Instance.DdTexturesAssets.ConvertAll(a => new TextureAsset(a.AssetName, a.IsProhibited, a.DefaultWidth, a.DefaultHeight, a.IsModelTexture, a.ModelBinding));

			DdAssets = DdModelBindingsAssets.Cast<AbstractAsset>()
				.Concat(DdModelsAssets.Cast<AbstractAsset>())
				.Concat(DdShadersAssets.Cast<AbstractAsset>())
				.Concat(DdTexturesAssets.Cast<AbstractAsset>())
				.ToList();

			AllAssets = AudioAudioAssets.Cast<AbstractAsset>()
				.Concat(CoreShadersAssets.Cast<AbstractAsset>())
				.Concat(DdAssets)
				.ToList();
		}

		public static AssetContainer Instance => _lazy.Value;

		public List<AudioAsset> AudioAudioAssets { get; }
		public List<ShaderAsset> CoreShadersAssets { get; }
		public List<ModelBindingAsset> DdModelBindingsAssets { get; }
		public List<ModelAsset> DdModelsAssets { get; }
		public List<ShaderAsset> DdShadersAssets { get; }
		public List<TextureAsset> DdTexturesAssets { get; }

		public List<AbstractAsset> DdAssets { get; }
		public List<AbstractAsset> AllAssets { get; }

		public List<AbstractAsset> GetAssets(BinaryType binaryType, string assetType)
		{
			string id = $"{binaryType.ToString().ToLower()}.{assetType.ToLower()}";

			return id switch
			{
				"audio.audio" => AudioAudioAssets.Cast<AbstractAsset>().ToList(),
				"core.shaders" => CoreShadersAssets.Cast<AbstractAsset>().ToList(),
				"dd.model bindings" => DdModelBindingsAssets.Cast<AbstractAsset>().ToList(),
				"dd.models" => DdModelsAssets.Cast<AbstractAsset>().ToList(),
				"dd.shaders" => DdShadersAssets.Cast<AbstractAsset>().ToList(),
				"dd.textures" => DdTexturesAssets.Cast<AbstractAsset>().ToList(),
				_ => throw new($"No asset data found for binary file type '{binaryType}' and asset type '{assetType}'."),
			};
		}

		public bool? IsProhibited(string name, AssetType assetType)
		{
			AbstractAsset? asset = AllAssets.Find(a => a.AssetName == name && a.AssetType == assetType);
			return asset?.IsProhibited;
		}
	}
}
