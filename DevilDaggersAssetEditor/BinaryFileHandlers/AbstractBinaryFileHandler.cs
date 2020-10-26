using DevilDaggersAssetEditor.Assets;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public abstract class AbstractBinaryFileHandler
	{
		protected AbstractBinaryFileHandler(BinaryFileType binaryFileType)
		{
			BinaryFileType = binaryFileType;
		}

		public BinaryFileType BinaryFileType { get; }

		public abstract void MakeBinary(List<AbstractAsset> allAssets, string outputPath, ProgressWrapper progress);

		public abstract void ExtractBinary(string inputPath, string outputPath, BinaryFileType binaryFileType, ProgressWrapper progress);

		public abstract void ValidateFile(byte[] sourceFileBytes);
	}
}