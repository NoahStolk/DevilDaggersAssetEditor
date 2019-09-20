using System;
using System.Collections.Generic;
using System.IO;
using DevilDaggersAssetCore.Assets;

namespace DevilDaggersAssetCore.BinaryFileHandlers
{
	public class ParticleFileHandler : AbstractBinaryFileHandler
	{
		private const string FolderName = "Particles";
		private const string FileExtension = ".bin";

		public ParticleFileHandler()
			: base(BinaryFileType.Particle)
		{
		}

		public override void Compress(List<AbstractAsset> allAssets, string outputPath)
		{

		}

		public override void Extract(string inputPath, string outputPath)
		{
			byte[] fileBuffer = File.ReadAllBytes(inputPath);

			Directory.CreateDirectory(Path.Combine(outputPath, FolderName));

			// Byte 0 - 3 = version?
			// Byte 4 - 7 = particle amount
			int i = 8;
			while (i < fileBuffer.Length)
			{
				string name = ReadNullTerminatedString(fileBuffer, i);
				i += name.Length;

				byte[] chunkBuffer = new byte[188];
				Buffer.BlockCopy(fileBuffer, i, chunkBuffer, 0, chunkBuffer.Length);

				i += 188;

				File.WriteAllBytes(Path.Combine(outputPath, FolderName, $"{name}{FileExtension}"), chunkBuffer);
			}
		}
	}
}