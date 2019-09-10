namespace DevilDaggersAssetCore.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public float Loudness { get; set; }

		public AudioAsset(string assetName, string description, string typeName, float loudness)
			: base(assetName, description, typeName)
		{
			Loudness = loudness;
		}
	}
}