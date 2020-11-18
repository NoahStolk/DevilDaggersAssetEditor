using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public class ParticleFileHandler : IBinaryFileHandler
	{
		/// <summary>
		/// uint magic, uint particle amount = 8 bytes.
		/// </summary>
		public const int HeaderSize = 8;
		public const int ParticleBufferLength = 188;

		public static readonly uint Magic1 = 4; // Maybe represents format version? Similar to survival file.

		public void MakeBinary(List<AbstractAsset> allAssets, string outputPath, ProgressWrapper progress)
		{
			progress.Report("Initializing 'particle' file creation.");

			allAssets = allAssets.Where(a => File.Exists(a.EditorPath)).ToList();

			byte[] fileBuffer;
			using (MemoryStream stream = new MemoryStream())
			{
				stream.Write(BitConverter.GetBytes(4), 0, sizeof(int));
				stream.Write(BitConverter.GetBytes(allAssets.Count), 0, sizeof(int));
				int i = 0;
				foreach (KeyValuePair<string, byte[]> kvp in GetChunks(allAssets))
				{
					progress.Report($"Writing file contents of \"{kvp.Key}\" to 'particle' file.", i++ / (float)allAssets.Count);

					stream.Write(Encoding.Default.GetBytes(kvp.Key), 0, kvp.Key.Length);
					stream.Write(kvp.Value, 0, kvp.Value.Length);
				}

				fileBuffer = stream.ToArray();
			}

			progress.Report("Writing 'particle' file.");
			File.WriteAllBytes(outputPath, fileBuffer);
		}

		private static Dictionary<string, byte[]> GetChunks(List<AbstractAsset> assets)
		{
			Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();
			foreach (AbstractAsset asset in assets)
				dict[asset.AssetName] = File.ReadAllBytes(asset.EditorPath);

			return dict;
		}

		public void ExtractBinary(string inputPath, string outputPath, ProgressWrapper progress)
		{
			byte[] fileBuffer = File.ReadAllBytes(inputPath);

			Directory.CreateDirectory(Path.Combine(outputPath, AssetType.Particle.GetFolderName()));

			int i = HeaderSize;
			while (i < fileBuffer.Length)
			{
				ParticleChunk chunk = ReadParticleChunk(fileBuffer, i);
				i += chunk.Buffer.Length;

				progress.Report($"Creating Particle file for chunk \"{chunk.Name}\".", i / (float)fileBuffer.Length);

				File.WriteAllBytes(Path.Combine(outputPath, AssetType.Particle.GetFolderName(), chunk.Name + AssetType.Particle.GetFileExtension()), chunk.Buffer[chunk.Name.Length..]);
			}
		}

		public void ValidateFile(byte[] sourceFileBytes)
		{
			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			if (magic1FromFile != Magic1)
				throw new($"Invalid file format. The magic number value is incorrect:\n\nHeader value 1: {magic1FromFile} should be {Magic1}");
		}

		public static ParticleChunk ReadParticleChunk(byte[] fileBuffer, int i)
		{
			string name = BinaryUtils.ReadNullTerminatedString(fileBuffer, i);

			byte[] buffer = new byte[ParticleBufferLength + name.Length];
			Buffer.BlockCopy(Encoding.Default.GetBytes(name), 0, buffer, 0, name.Length);
			Buffer.BlockCopy(fileBuffer, i + name.Length, buffer, name.Length, ParticleBufferLength);

			return new ParticleChunk(name, (uint)i, (uint)buffer.Length, buffer);
		}
	}
}