using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 0;
		public override byte ColorB => 255;

		[JsonProperty]
		public float Loudness { get; set; }
		[JsonProperty]
		public bool PresentInDefaultLoudness { get; set; }

		public AudioAsset(string assetName, string description, string chunkTypeName, float loudness, bool presentInDefaultLoudness)
			: base(assetName, description, chunkTypeName)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;
		}

		public override AbstractUserAsset ToUserAsset()
		{
			return new AudioUserAsset(AssetName, EditorPath, Loudness);
		}

		public override void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			base.ImportValuesFromUserAsset(userAsset);

			if (userAsset is AudioUserAsset audioUserAsset)
				Loudness = audioUserAsset.Loudness;
		}
	}
}