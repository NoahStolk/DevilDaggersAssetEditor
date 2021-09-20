using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersCore.Mods;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelAsset : AbstractAsset
	{
		public ModelAsset(string assetName, bool isProhibited, int defaultIndexCount, int defaultVertexCount)
			: base(assetName, AssetType.Model, isProhibited)
		{
			DefaultIndexCount = defaultIndexCount;
			DefaultVertexCount = defaultVertexCount;
		}

		public int DefaultIndexCount { get; }
		public int DefaultVertexCount { get; }

		public override UserAsset ToUserAsset()
			=> new(AssetType.Model, AssetName, EditorPath);
	}
}
