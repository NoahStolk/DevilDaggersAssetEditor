using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;
using System;
using System.Drawing;

namespace DevilDaggersAssetCore.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TextureAsset : AbstractAsset
	{
		[JsonProperty]
		public Point DefaultDimensions { get; }
		[JsonProperty]
		public string ModelBinding { get; set; }
		[JsonProperty]
		public bool IsModelTexture { get; set; }

		public TextureAsset(string assetName, string description, string[] tags, string chunkTypeName, Point defaultDimensions, string modelBinding, bool isModelTexture)
			: base(assetName, description, tags, chunkTypeName)
		{
			DefaultDimensions = defaultDimensions;
			ModelBinding = modelBinding;
			IsModelTexture = isModelTexture;
		}

		public override AbstractUserAsset ToUserAsset() => new TextureUserAsset(AssetName, EditorPath);

		public static byte GetMipmapCount(int width, int height) => (byte)(Math.Log(Math.Min(width, height), 2) + 1);
	}
}