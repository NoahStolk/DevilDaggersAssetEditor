using System;
using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets
{
	public class ModelBinaryAsset : BinaryAsset
	{
		public ModelBinaryAsset(string assetName)
			: base(assetName)
		{
		}

		public override byte Type => 0x01;

		public override string FileExtension => "obj";

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