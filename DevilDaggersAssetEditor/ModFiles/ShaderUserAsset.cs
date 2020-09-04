using DevilDaggersAssetCore.Chunks;

namespace DevilDaggersAssetCore.ModFiles
{
	public class ShaderUserAsset : AbstractUserAsset
	{
		public override string ChunkTypeName => nameof(ShaderChunk);

		public ShaderUserAsset(string assetName, string editorPath)
			: base(assetName, editorPath)
		{
		}
	}
}