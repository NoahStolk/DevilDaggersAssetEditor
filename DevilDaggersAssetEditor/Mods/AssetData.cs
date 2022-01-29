namespace DevilDaggersAssetEditor.Mods;

public class AssetData
{
	public AssetData(string assetName, bool isProhibited)
	{
		AssetName = assetName;
		IsProhibited = isProhibited;
	}

	public string AssetName { get; }
	public bool IsProhibited { get; }

	public AssetType AssetType { get; set; }
}
