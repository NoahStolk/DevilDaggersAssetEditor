using DevilDaggersAssetEditor.ModFiles;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelAsset : AbstractAsset
	{
		public ModelAsset(string assetName, string description, string[] tags, string chunkTypeName, int defaultVertexCount, int defaultIndexCount)
			: base(assetName, description, tags, chunkTypeName)
		{
			DefaultVertexCount = defaultVertexCount;
			DefaultIndexCount = defaultIndexCount;
		}

		public int DefaultVertexCount { get; set; }
		public int DefaultIndexCount { get; set; }

		public override AbstractUserAsset ToUserAsset()
			=> new ModelUserAsset(AssetName, EditorPath);
	}
}