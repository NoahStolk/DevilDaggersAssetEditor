using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Assets
{
	public abstract class AbstractAsset
	{
		protected AbstractAsset(string assetName, string description, string[] tags, string chunkTypeName)
		{
			AssetName = assetName;
			Description = description;
			Tags = tags;
			ChunkTypeName = chunkTypeName;
		}

		public string AssetName { get; }
		public string Description { get; }
		public string[] Tags { get; }
		public string ChunkTypeName { get; }

		[JsonIgnore]
		public string EditorPath { get; set; } = GuiUtils.FileNotFound;

		public abstract AbstractUserAsset ToUserAsset();

		public virtual void ImportValuesFromUserAsset(AbstractUserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}