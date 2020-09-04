using DevilDaggersAssetEditor.ModFiles;

namespace DevilDaggersAssetEditor.Assets
{
	public class ShaderAsset : AbstractAsset
	{
		public ShaderAsset(string assetName, string description, string[] tags, string chunkTypeName)
			: base(assetName, description, tags, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ShaderUserAsset(AssetName, EditorPath);
	}
}