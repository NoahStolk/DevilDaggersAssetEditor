using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersCore.Mods;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public AudioAsset(string assetName, string description, bool isProhibited, List<string> tags, float loudness, bool presentInDefaultLoudness)
			: base(assetName, AssetType.Audio, description, isProhibited, tags)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;

			DefaultLoudness = loudness;
		}

		public float Loudness { get; set; }

		public bool PresentInDefaultLoudness { get; }

		[JsonIgnore]
		public float DefaultLoudness { get; }

		public override UserAsset ToUserAsset()
			=> new AudioUserAsset(AssetName, EditorPath, Loudness);

		public override void ImportValuesFromUserAsset(UserAsset userAsset)
		{
			base.ImportValuesFromUserAsset(userAsset);

			if (userAsset is AudioUserAsset audioUserAsset)
				Loudness = audioUserAsset.Loudness;
		}
	}
}
