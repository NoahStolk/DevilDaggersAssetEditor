namespace DevilDaggersAssetEditor.Code.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public float Loudness { get; set; }

		public AudioAsset(string assetName, string description, float loudness)
			: base(assetName, description)
		{
			Loudness = loudness;
		}
	}
}