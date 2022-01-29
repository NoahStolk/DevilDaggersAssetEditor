namespace DevilDaggersAssetEditor.Mods;

public class TextureAssetData : AssetData
{
	public TextureAssetData(string assetName, bool isProhibited, int defaultWidth, int defaultHeight, bool isModelTexture, string modelBinding)
		: base(assetName, isProhibited)
	{
		DefaultWidth = defaultWidth;
		DefaultHeight = defaultHeight;
		IsModelTexture = isModelTexture;
		ModelBinding = modelBinding;
	}

	public int DefaultWidth { get; }
	public int DefaultHeight { get; }
	public bool IsModelTexture { get; }
	public string ModelBinding { get; }
}
