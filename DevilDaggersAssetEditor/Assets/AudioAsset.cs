using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersCore.Mods;
using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public AudioAsset(string assetName, bool isProhibited, float loudness, bool presentInDefaultLoudness)
			: base(assetName, AssetType.Audio, isProhibited)
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
