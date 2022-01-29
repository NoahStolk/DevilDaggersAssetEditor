namespace DevilDaggersAssetEditor.Mods;

public class ModelAssetData : AssetData
{
	public ModelAssetData(string assetName, bool isProhibited, int defaultIndexCount, int defaultVertexCount)
		: base(assetName, isProhibited)
	{
		DefaultIndexCount = defaultIndexCount;
		DefaultVertexCount = defaultVertexCount;
	}

	public int DefaultIndexCount { get; }
	public int DefaultVertexCount { get; }
}
