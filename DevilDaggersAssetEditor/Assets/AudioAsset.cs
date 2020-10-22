using DevilDaggersAssetEditor.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public AudioAsset(string assetName, string description, string[] tags, string chunkTypeName, float loudness, bool presentInDefaultLoudness)
			: base(assetName, description, tags, chunkTypeName)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;

			DefaultLoudness = loudness;
		}

		public float Loudness { get; set; }
		public bool PresentInDefaultLoudness { get; }

		[JsonIgnore]
		public float DefaultLoudness { get; }

		public override AbstractUserAsset ToUserAsset()
			=> new AudioUserAsset(AssetName, EditorPath, Loudness);

		public override void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			base.ImportValuesFromUserAsset(userAsset);

			if (userAsset is AudioUserAsset audioUserAsset)
				Loudness = audioUserAsset.Loudness;
		}
	}
}