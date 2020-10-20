using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets
{
	public class AudioBinaryAsset : BinaryAsset
	{
		public AudioBinaryAsset(string assetName)
			: base(assetName)
		{
		}

		public override byte Type => 0x20;

		public override string FileExtension => "wav";

		public override void Construct(ImmutableArray<byte> fileContents)
			=> Contents = fileContents;

		public override FileResult[] Extract()
			=> new[] { new FileResult(AssetName, Contents) };
	}
}