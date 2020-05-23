using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	public class ModelAsset : AbstractAsset
	{
		[JsonProperty]
		public int DefaultVertexCount { get; set; }
		[JsonProperty]
		public int DefaultIndexCount { get; set; }

		public ModelAsset(string assetName, string description, string[] tags, string chunkTypeName, int defaultVertexCount, int defaultIndexCount)
			: base(assetName, description, tags, chunkTypeName)
		{
			DefaultVertexCount = defaultVertexCount;
			DefaultIndexCount = defaultIndexCount;
		}

		public override AbstractUserAsset ToUserAsset() => new ModelUserAsset(AssetName, EditorPath);
	}
}