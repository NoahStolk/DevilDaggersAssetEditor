using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets
{
	public abstract class BinaryAsset
	{
		protected BinaryAsset(string assetName)
		{
			AssetName = assetName;
		}

		public string AssetName { get; }

		public uint StartOffset { get; set; }
		public uint Size { get; set; }

		public ImmutableArray<byte> Contents { get; set; }

		public abstract byte Type { get; }
		public abstract string FileExtension { get; }

		public abstract void Construct(ImmutableArray<byte> fileContents);

		public abstract FileResult[] Extract();
	}
}