using System;
using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets
{
	public class ShaderBinaryAsset : BinaryAsset
	{
		public ShaderBinaryAsset(string assetName)
			: base(assetName)
		{
		}

		public override byte Type => 0x10;

		public override string FileExtension => "glsl";

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