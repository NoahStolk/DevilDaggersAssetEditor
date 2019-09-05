namespace DevilDaggersAssetEditor.Code.Assets
{
	public class AudioAsset : AbstractAsset
	{
		public float Loudness { get; set; }

		public AudioAsset(string fileName, string description, float loudness)
			: base(fileName, description)
		{
			Loudness = loudness;
		}
	}
}