using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class AbstractAsset
	{
		[JsonProperty]
		public string AssetName { get; }
		[JsonProperty]
		public string Description { get; }
		[JsonProperty]
		public string ChunkTypeName { get; }

		public string EditorPath { get; set; } = "<NONE SPECIFIED>";

		protected AbstractAsset(string assetName, string description, string chunkTypeName)
		{
			AssetName = assetName;
			Description = description;
			ChunkTypeName = chunkTypeName;
		}

		public abstract AbstractUserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}