using DevilDaggersAssetCore.Assets;
using System;
using System.Collections.Generic;

namespace DevilDaggersAssetCore.BinaryFileHandlers
{
	public abstract class AbstractBinaryFileHandler
	{
		public BinaryFileType BinaryFileType { get; }

		protected AbstractBinaryFileHandler(BinaryFileType binaryFileType)
		{
			BinaryFileType = binaryFileType;
		}

		public abstract void Compress(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription);

		public abstract void Extract(string inputPath, string outputPath, BinaryFileType binaryFileType, Progress<float> progress, Progress<string> progressDescription);
	}
}