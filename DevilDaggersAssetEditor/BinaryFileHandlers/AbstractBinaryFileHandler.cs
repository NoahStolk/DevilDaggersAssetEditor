using DevilDaggersAssetEditor.Assets;
using System;
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

		public abstract void MakeBinary(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription);

		public abstract void ExtractBinary(string inputPath, string outputPath, BinaryFileType binaryFileType, Progress<float> progress, Progress<string> progressDescription);

		public abstract void ValidateFile(byte[] sourceFileBytes);
	}
}