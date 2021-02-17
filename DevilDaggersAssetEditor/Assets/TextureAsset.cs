using DevilDaggersAssetEditor.ModFiles;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DevilDaggersAssetEditor.Assets
{
	public class TextureAsset : AbstractAsset
	{
		public TextureAsset(string assetName, string description, List<string> tags, Point defaultDimensions, string modelBinding, bool isModelTexture)
			: base(assetName, AssetType.Texture, description, tags)
		{
			DefaultDimensions = defaultDimensions;
			ModelBinding = modelBinding;
			IsModelTexture = isModelTexture;
		}

		public Point DefaultDimensions { get; }
		public string ModelBinding { get; set; }
		public bool IsModelTexture { get; set; }

		public override UserAsset ToUserAsset()
			=> new(AssetType.Texture, AssetName, EditorPath);

		public static byte GetMipmapCount(int width, int height)
			=> (byte)(Math.Log(Math.Min(width, height), 2) + 1);
	}
}
