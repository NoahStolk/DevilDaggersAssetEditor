using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			allAssets = allAssets.Where(a => a.EditorPath.IsPathValid()).ToList();

			byte[] fileBuffer;
			using (MemoryStream stream = new MemoryStream())
			{
				stream.Write(BitConverter.GetBytes(4), 0, sizeof(int));
				stream.Write(BitConverter.GetBytes(allAssets.Count), 0, sizeof(int));
				foreach (KeyValuePair<string, byte[]> kvp in GetChunks(allAssets))
				{
					stream.Write(Encoding.Default.GetBytes(kvp.Key), 0, kvp.Key.Length);
					stream.Write(kvp.Value, 0, kvp.Value.Length);
				}
				fileBuffer = stream.ToArray();
			}

			File.WriteAllBytes(outputPath, fileBuffer);
		}

		private Dictionary<string, byte[]> GetChunks(List<AbstractAsset> assets)
		{
			Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();

			foreach (AbstractAsset asset in assets)
				dict[asset.AssetName] = File.ReadAllBytes(asset.EditorPath);

			return dict;
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