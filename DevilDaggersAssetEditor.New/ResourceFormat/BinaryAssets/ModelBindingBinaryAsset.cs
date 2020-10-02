using System;
using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets
{
	public class ModelBindingBinaryAsset : BinaryAsset
	{
		public ModelBindingBinaryAsset(string assetName)
			: base(assetName)
		{
		}

		public override byte Type => 0x80;

		public override string FileExtension => "txt";

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