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

		public virtual GenericUserAsset ToUserAsset()
		{
			return new GenericUserAsset(AssetName, EditorPath);
		}

		public virtual void ImportValuesFromUserAsset(GenericUserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}