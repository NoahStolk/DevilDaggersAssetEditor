using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AudioAsset : AbstractAsset
	{
		[JsonProperty]
		public float Loudness { get; set; }
		[JsonProperty]
		public bool PresentInDefaultLoudness { get; }

		public float DefaultLoudness { get; }

		public AudioAsset(string assetName, string description, string[] tags, string chunkTypeName, float loudness, bool presentInDefaultLoudness)
			: base(assetName, description, tags, chunkTypeName)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;

			DefaultLoudness = loudness;
		}

		public override AbstractUserAsset ToUserAsset() => new AudioUserAsset(AssetName, EditorPath, Loudness);

		public override void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			base.ImportValuesFromUserAsset(userAsset);

			if (userAsset is AudioUserAsset audioUserAsset)
				Loudness = audioUserAsset.Loudness;
		}
	}
}