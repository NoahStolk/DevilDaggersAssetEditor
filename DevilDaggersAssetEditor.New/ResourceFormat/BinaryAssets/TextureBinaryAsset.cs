using System;
using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets
{
	public class TextureBinaryAsset : BinaryAsset
	{
		public TextureBinaryAsset(string assetName)
			: base(assetName)
		{
		}

		public override byte Type => 0x02;

		public override string FileExtension => "png";

		public override void Construct(ImmutableArray<byte> fileContents)
		{
			throw new NotImplementedException();
		}

		public override FileResult[] Extract()
		{
			throw new NotImplementedException();
		}
	}
}