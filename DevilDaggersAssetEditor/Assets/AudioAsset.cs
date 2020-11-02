using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public AudioAsset(string assetName, string description, List<string> tags, float loudness, bool presentInDefaultLoudness)
			: base(assetName, AssetType.Audio, description, tags)
		{
			PresentInDefaultLoudness = presentInDefaultLoudness;
			DefaultLoudness = loudness;

			Loudness = loudness;
		}

		public bool PresentInDefaultLoudness { get; }
		public float DefaultLoudness { get; }

		public float Loudness { get; set; }

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