using DevilDaggersAssetEditor.Assets;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public interface IBinaryFileHandler
	{
		void MakeBinary(List<AbstractAsset> allAssets, string outputPath, ProgressWrapper progress);

		void ExtractBinary(string inputPath, string outputPath, BinaryFileType binaryFileType, ProgressWrapper progress);

		void ValidateFile(byte[] sourceFileBytes);
	}
}