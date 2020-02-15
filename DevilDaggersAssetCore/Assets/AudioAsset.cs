using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AudioAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 0;
		public override byte ColorB => 255;

		[JsonProperty]
		public float Loudness { get; set; }
		[JsonProperty]
		public bool PresentInDefaultLoudness { get; }

		public float DefaultLoudness { get; }

		public AudioAsset(string assetName, string description, string entityName, string chunkTypeName, float loudness, bool presentInDefaultLoudness)
			: base(assetName, description, entityName, chunkTypeName)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;

			DefaultLoudness = loudness;
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