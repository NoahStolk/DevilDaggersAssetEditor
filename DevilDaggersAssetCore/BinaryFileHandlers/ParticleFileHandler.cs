using DevilDaggersAssetCore.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetCore.BinaryFileHandlers
{
	public class ParticleFileHandler : AbstractBinaryFileHandler
	{
		private const string folderName = "Particles";
		private const string fileExtension = ".bin";
		private const int particleBufferLength = 188;

		public ParticleFileHandler()
			: base(BinaryFileType.Particle)
		{
		}

		public override void Compress(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription)
		{
			((IProgress<string>)progressDescription).Report("Initializing 'particle' file creation.");

			allAssets = allAssets.Where(a => a.EditorPath.IsPathValid()).ToList();

			byte[] fileBuffer;
			using (MemoryStream stream = new MemoryStream())
			{
				stream.Write(BitConverter.GetBytes(4), 0, sizeof(int));
				stream.Write(BitConverter.GetBytes(allAssets.Count), 0, sizeof(int));
				int i = 0;
				foreach (KeyValuePair<string, byte[]> kvp in GetChunks(allAssets))
				{
					((IProgress<float>)progress).Report(i++ / (float)allAssets.Count);
					((IProgress<string>)progressDescription).Report($"Writing file contents of \"{kvp.Key}\" to 'particle' file.");

					stream.Write(Encoding.Default.GetBytes(kvp.Key), 0, kvp.Key.Length);
					stream.Write(kvp.Value, 0, kvp.Value.Length);
				}
				fileBuffer = stream.ToArray();
			}

			((IProgress<string>)progressDescription).Report("Writing 'particle' file.");
			File.WriteAllBytes(outputPath, fileBuffer);
		}

		private Dictionary<string, byte[]> GetChunks(List<AbstractAsset> assets)
		{
			Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();

			foreach (AbstractAsset asset in assets)
				dict[asset.AssetName] = File.ReadAllBytes(asset.EditorPath);

			return dict;
		}

		public override void Extract(string inputPath, string outputPath, BinaryFileType binaryFileType, Progress<float> progress, Progress<string> progressDescription)
		{
			byte[] fileBuffer = File.ReadAllBytes(inputPath);

			Directory.CreateDirectory(Path.Combine(outputPath, folderName));

			// Byte 0 - 3 = version?
			// Byte 4 - 7 = particle amount
			int i = 8;
			while (i < fileBuffer.Length)
			{
				string name = Utils.ReadNullTerminatedString(fileBuffer, i);
				i += name.Length;

				((IProgress<float>)progress).Report(i / (float)fileBuffer.Length);
				((IProgress<string>)progressDescription).Report($"Creating Particle file for chunk \"{name}\".");

				byte[] chunkBuffer = new byte[particleBufferLength];
				Buffer.BlockCopy(fileBuffer, i, chunkBuffer, 0, chunkBuffer.Length);
				i += particleBufferLength;

				File.WriteAllBytes(Path.Combine(outputPath, folderName, $"{name}{fileExtension}"), chunkBuffer);
			}

			// Create mod file.
			if (settings.CreateModFileWhenExtracting)
				CreateModFile(outputPath, binaryFileType);

			// Open the output path.
			if (settings.OpenModFolderAfterExtracting)
				Process.Start(outputPath);
		}
	}
}