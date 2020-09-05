using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class AbstractAsset
	{
		protected AbstractAsset(string assetName, string description, string[] tags, string chunkTypeName)
		{
			AssetName = assetName;
			Description = description;
			Tags = tags;
			ChunkTypeName = chunkTypeName;
		}

		[JsonProperty]
		public string AssetName { get; }
		[JsonProperty]
		public string Description { get; }
		[JsonProperty]
		public string[] Tags { get; }
		[JsonProperty]
		public string ChunkTypeName { get; }

		public string EditorPath { get; set; } = GuiUtils.FileNotFound;

		public abstract AbstractUserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}