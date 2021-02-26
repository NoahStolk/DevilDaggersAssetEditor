using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ModelAsset : AbstractAsset
	{
		public ModelAsset(string assetName, string description, bool isProhibited, List<string> tags, int defaultIndexCount, int defaultVertexCount)
			: base(assetName, AssetType.Model, description, isProhibited, tags)
		{
			DefaultIndexCount = defaultIndexCount;
			DefaultVertexCount = defaultVertexCount;
		}

		public int DefaultIndexCount { get; }

		public int DefaultVertexCount { get; }

		public override UserAsset ToUserAsset()
			=> new(AssetType.Model, AssetName, EditorPath);
	}
}
