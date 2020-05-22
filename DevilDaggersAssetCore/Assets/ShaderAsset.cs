using DevilDaggersAssetCore.ModFiles;

namespace DevilDaggersAssetCore.Assets
{
	public class ShaderAsset : AbstractAsset
	{
		public ShaderAsset(string assetName, string description, string entityName, string chunkTypeName)
			: base(assetName, description, entityName, chunkTypeName)
		{
		}

		public override AbstractUserAsset ToUserAsset() => new ShaderUserAsset(AssetName, EditorPath);
	}
}