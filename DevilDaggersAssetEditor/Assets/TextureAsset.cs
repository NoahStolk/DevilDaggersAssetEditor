using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersCore.Mods;
using System;

namespace DevilDaggersAssetEditor.Assets;

public class TextureAsset : AbstractAsset
{
	public TextureAsset(string assetName, bool isProhibited, int defaultWidth, int defaultHeight, bool isModelTexture, string modelBinding)
		: base(assetName, AssetType.Texture, isProhibited)
	{
		DefaultWidth = defaultWidth;
		DefaultHeight = defaultHeight;
		IsModelTexture = isModelTexture;
		ModelBinding = modelBinding;
	}

	public int DefaultWidth { get; }
	public int DefaultHeight { get; }
	public bool IsModelTexture { get; set; }
	public string ModelBinding { get; set; }

	public override UserAsset ToUserAsset()
		=> new(AssetType.Texture, AssetName, EditorPath);

	public static byte GetMipmapCount(int width, int height)
		=> (byte)(Math.Log(Math.Min(width, height), 2) + 1);
}
