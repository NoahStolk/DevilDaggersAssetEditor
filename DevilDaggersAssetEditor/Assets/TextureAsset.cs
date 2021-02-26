using DevilDaggersAssetEditor.ModFiles;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DevilDaggersAssetEditor.Assets
{
	public class TextureAsset : AbstractAsset
	{
		public TextureAsset(string assetName, string description, bool isProhibited, List<string> tags, Point defaultDimensions, bool isModelTexture, string modelBinding)
			: base(assetName, AssetType.Texture, description, isProhibited, tags)
		{
			DefaultDimensions = defaultDimensions;
			IsModelTexture = isModelTexture;
			ModelBinding = modelBinding;
		}

		public Point DefaultDimensions { get; }
		public bool IsModelTexture { get; set; }
		public string ModelBinding { get; set; }

		public override UserAsset ToUserAsset()
			=> new(AssetType.Texture, AssetName, EditorPath);

		public static byte GetMipmapCount(int width, int height)
			=> (byte)(Math.Log(Math.Min(width, height), 2) + 1);
	}
}
