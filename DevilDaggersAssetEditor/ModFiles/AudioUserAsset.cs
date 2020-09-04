using DevilDaggersAssetCore.Chunks;

namespace DevilDaggersAssetCore.ModFiles
{
	public class AudioUserAsset : AbstractUserAsset
	{
		public override string ChunkTypeName => nameof(AudioChunk);

		public float Loudness { get; }

		public AudioUserAsset(string assetName, string editorPath, float loudness)
			: base(assetName, editorPath)
		{
			Loudness = loudness;
		}
	}
}