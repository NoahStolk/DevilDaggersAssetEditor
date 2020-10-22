using DevilDaggersAssetEditor.ModFiles;
using Newtonsoft.Json;
using System;
using System.Drawing;

namespace DevilDaggersAssetEditor.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TextureAsset : AbstractAsset
	{
		public TextureAsset(string assetName, string description, string[] tags, string chunkTypeName, Point defaultDimensions, string modelBinding, bool isModelTexture)
			: base(assetName, description, tags, chunkTypeName)
		{
			DefaultDimensions = defaultDimensions;
			ModelBinding = modelBinding;
			IsModelTexture = isModelTexture;
		}

		public Point DefaultDimensions { get; }
		public string ModelBinding { get; set; }
		public bool IsModelTexture { get; set; }

		public override AbstractUserAsset ToUserAsset()
			=> new TextureUserAsset(AssetName, EditorPath);

		public static byte GetMipmapCount(int width, int height)
			=> (byte)(Math.Log(Math.Min(width, height), 2) + 1);
	}
}