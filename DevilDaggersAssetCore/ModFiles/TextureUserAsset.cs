using DevilDaggersAssetCore.Chunks;

namespace DevilDaggersAssetCore.ModFiles
{
	public class TextureUserAsset : AbstractUserAsset
	{
		public override string ChunkTypeName => nameof(TextureChunk);

		public TextureUserAsset(string assetName, string editorPath)
			: base(assetName, editorPath)
		{
		}
	}
}