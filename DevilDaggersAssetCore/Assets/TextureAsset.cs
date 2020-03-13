using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;
using System;
using System.Drawing;

namespace DevilDaggersAssetCore.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TextureAsset : AbstractAsset
	{
		public override byte ColorR => 255;
		public override byte ColorG => 127;
		public override byte ColorB => 0;

		[JsonProperty]
		public Point DefaultDimensions { get; }
		[JsonProperty]
		public string ModelBinding { get; set; }

		public TextureAsset(string assetName, string description, string chunkTypeName, string entityName, Point defaultDimensions, string modelBinding)
			: base(assetName, description, entityName, chunkTypeName)
		{
			DefaultDimensions = defaultDimensions;
			ModelBinding = modelBinding;
		}

		public override AbstractUserAsset ToUserAsset()
		{
			return new TextureUserAsset(AssetName, EditorPath);
		}

		public static byte GetMipmapCount(int width, int height)
		{
			return (byte)(Math.Log(Math.Min(width, height), 2) + 1);
		}
	}
}