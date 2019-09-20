using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	public class AudioAsset : AbstractAsset
	{
		[JsonProperty]
		public float Loudness { get; set; }
		[JsonProperty]
		public bool PresentInDefaultLoudness { get; set; }

		public AudioAsset(string assetName, string description, string typeName, float loudness, bool presentInDefaultLoudness)
			: base(assetName, description, typeName)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;
		}

		public override GenericUserAsset ToUserAsset()
		{
			return new AudioUserAsset(AssetName, EditorPath, Loudness);
		}

		public override void ImportValuesFromUserAsset(GenericUserAsset userAsset)
		{
			base.ImportValuesFromUserAsset(userAsset);

			if (userAsset is AudioUserAsset audioUserAsset)
				Loudness = audioUserAsset.Loudness;
		}
	}
}