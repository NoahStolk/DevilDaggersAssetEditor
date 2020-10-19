namespace DevilDaggersAssetEditor.New.ResourceFormat
{
	public abstract class ResourceFileAsset
	{
		protected ResourceFileAsset(ResourceAssetType type, string assetName)
		{
			Type = type;
			AssetName = assetName;
		}

		public ResourceAssetType Type { get; }
		public string AssetName { get; }

		public string? CurrentPath { get; set; }
	}
}