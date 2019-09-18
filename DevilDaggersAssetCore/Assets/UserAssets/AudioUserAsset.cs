namespace DevilDaggersAssetCore.Assets.UserAssets
{
	public class AudioUserAsset : GenericUserAsset
	{
		public float Loudness { get; }

		public AudioUserAsset(string assetName, string editorPath, float loudness)
			: base(assetName, editorPath)
		{
			Loudness = loudness;
		}
	}
}