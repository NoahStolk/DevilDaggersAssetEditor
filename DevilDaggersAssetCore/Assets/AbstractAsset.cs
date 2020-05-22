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
		public string Description { get; set; }
		[JsonProperty]
		public string EntityName { get; set; }
		[JsonProperty]
		public string ChunkTypeName { get; }

		public string EditorPath { get; set; } = Utils.FileNotFound;

		protected AbstractAsset(string assetName, string description, string entityName, string chunkTypeName)
		{
			AssetName = assetName;
			Description = description;
			EntityName = entityName; // TODO: Link to DevilDaggersCore.Game.DevilDaggersEntity to do more cool stuff...
			ChunkTypeName = chunkTypeName;
		}

		public abstract AbstractUserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}