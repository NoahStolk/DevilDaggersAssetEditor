using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	public class ModelAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 0;
		public override byte ColorB => 0;

		[JsonProperty]
		public int DefaultVertexCount { get; set; }
		[JsonProperty]
		public int DefaultIndexCount { get; set; }

		public ModelAsset(string assetName, string description, string entityName, string chunkTypeName, int defaultVertexCount, int defaultIndexCount)
			: base(assetName, description, entityName, chunkTypeName)
		{
			DefaultVertexCount = defaultVertexCount;
			DefaultIndexCount = defaultIndexCount;
		}

		public override AbstractUserAsset ToUserAsset() => new ModelUserAsset(AssetName, EditorPath);
	}
}