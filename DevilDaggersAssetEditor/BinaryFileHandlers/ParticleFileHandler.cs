using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Headers;
using DevilDaggersAssetEditor.User;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public class ParticleFileHandler : AbstractBinaryFileHandler
	{
		/// <summary>
		/// uint magic, uint particle amount = 8 bytes.
		/// </summary>
		public const int HeaderSize = 8;
		public const int ParticleBufferLength = 188;

		public static readonly uint Magic1 = 4; // Maybe represents format version? Similar to survival file.

		private const string folderName = "Particles";
		private const string fileExtension = ".bin";

		public ParticleFileHandler()
			: base(BinaryFileType.Particle)
		{
		}

		public override void MakeBinary(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription)
		{
			((IProgress<string>)progressDescription).Report("Initializing 'particle' file creation.");

			allAssets = allAssets.Where(a => File.Exists(a.EditorPath)).ToList();

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

		public override void ExtractBinary(string inputPath, string outputPath, BinaryFileType binaryFileType, Progress<float> progress, Progress<string> progressDescription)
		{
			byte[] fileBuffer = File.ReadAllBytes(inputPath);

			Directory.CreateDirectory(Path.Combine(outputPath, folderName));

			int i = HeaderSize;
			while (i < fileBuffer.Length)
			{
				ParticleChunk chunk = ReadParticleChunk(fileBuffer, i);
				i += chunk.Name.Length;
				i += chunk.Buffer.Length;

				((IProgress<float>)progress).Report(i / (float)fileBuffer.Length);
				((IProgress<string>)progressDescription).Report($"Creating Particle file for chunk \"{chunk.Name}\".");

				File.WriteAllBytes(Path.Combine(outputPath, folderName, $"{chunk.Name}{fileExtension}"), chunk.Buffer);
			}

			// Create mod file.
			if (UserHandler.Instance.Settings.CreateModFileWhenExtracting)
				CreateModFile(outputPath, binaryFileType);

			// Open the output path.
			if (UserHandler.Instance.Settings.OpenModFolderAfterExtracting)
				Process.Start(outputPath);
		}

		public override void ValidateFile(byte[] sourceFileBytes)
		{
			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			if (magic1FromFile != Magic1)
				throw new Exception($"Invalid file format. The magic number value is incorrect:\n\nHeader value 1: {magic1FromFile} should be {Magic1}");
		}

		public ParticleChunk ReadParticleChunk(byte[] fileBuffer, int i)
		{
			string name = Utils.ReadNullTerminatedString(fileBuffer, i);

			byte[] chunkBuffer = new byte[ParticleBufferLength];
			Buffer.BlockCopy(fileBuffer, i + name.Length, chunkBuffer, 0, chunkBuffer.Length);

			return new ParticleChunk(name, (uint)(i + name.Length), (uint)chunkBuffer.Length) { Buffer = chunkBuffer, Header = new ParticleHeader(Encoding.Default.GetBytes(name)) };
		}
	}
}