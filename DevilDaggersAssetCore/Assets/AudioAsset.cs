namespace DevilDaggersAssetCore.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public float Loudness { get; set; }
		public bool PresentInDefaultLoudness { get; set; }

		public AudioAsset(string assetName, string description, string typeName, float loudness, bool presentInDefaultLoudness)
			: base(assetName, description, typeName)
		{
			Loudness = loudness;
			PresentInDefaultLoudness = presentInDefaultLoudness;
		}
	}
}