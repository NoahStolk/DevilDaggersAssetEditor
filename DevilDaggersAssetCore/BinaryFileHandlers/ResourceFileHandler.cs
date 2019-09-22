using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Chunks;
using DevilDaggersAssetCore.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetCore.BinaryFileHandlers
{
	public class ResourceFileHandler : AbstractBinaryFileHandler
	{
		public const int HeaderSize = 12;

		public static readonly ulong Magic1;
		public static readonly ulong Magic2;

		static ResourceFileHandler()
		{
			Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
			Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

			static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d) => a | b << 8 | c << 16 | d << 24;
		}

		public ResourceFileHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
			if (binaryFileType == BinaryFileType.Particle)
				throw new Exception($"{nameof(BinaryFileType.Particle)} is unsupported by {nameof(ResourceFileHandler)}, use {nameof(ParticleFileHandler)} instead.");
		}

		/// <summary>
		/// Compresses multiple asset files into one binary file that can be read by Devil Daggers.
		/// </summary>
		/// <param name="allAssets">The list of asset objects.</param>
		/// <param name="outputPath">The path where the compressed binary file will be placed.</param>
		public override void Compress(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription)
		{
			((IProgress<string>)progressDescription).Report($"Initializing '{BinaryFileType.ToString().ToLower()}' file creation.");

			string binaryFileTypeName = BinaryFileType.ToString().ToLower();
			allAssets = allAssets.Where(a => a.EditorPath.IsPathValid()).ToList();

			((IProgress<string>)progressDescription).Report("Generating chunks based on asset list.");
			Dictionary<ChunkInfo, List<AbstractChunk>> chunkCollections = GetChunks(allAssets, progress, progressDescription, out int chunkCount);

			// Create TOC stream.
			((IProgress<string>)progressDescription).Report("Generating TOC stream.");
			byte[] tocBuffer;
			Dictionary<AbstractChunk, long> startOffsetBytePositions = new Dictionary<AbstractChunk, long>();
			using (MemoryStream tocStream = new MemoryStream())
			{
				foreach (KeyValuePair<ChunkInfo, List<AbstractChunk>> chunkCollection in chunkCollections)
				{
					ushort type = chunkCollection.Key.BinaryTypes[0]; // TODO: Shaders have multiple types.

					foreach (AbstractChunk chunk in chunkCollection.Value)
					{
						// Write asset type.
						tocStream.Write(BitConverter.GetBytes(type), 0, sizeof(byte));
						tocStream.Position++;

						// Write name.
						tocStream.Write(Encoding.Default.GetBytes(chunk.Name), 0, chunk.Name.Length);
						tocStream.Position++;

						// Write start offsets when TOC buffer size is defined.
						startOffsetBytePositions[chunk] = tocStream.Position;
						tocStream.Position += sizeof(uint);

						// Write size.
						tocStream.Write(BitConverter.GetBytes(chunk.Size), 0, sizeof(uint));

						// TODO: Figure out unknown value and write...
						// No reason to write anything for now.
						tocStream.Position += sizeof(uint);
					}
				}
				tocStream.Write(BitConverter.GetBytes(0), 0, 2); // Empty value between TOC and assets.
				tocBuffer = tocStream.ToArray();
			}

			// Create asset stream.
			((IProgress<string>)progressDescription).Report("Generating asset stream.");
			byte[] assetBuffer;
			using (MemoryStream assetStream = new MemoryStream())
			{
				int i = 0;
				foreach (KeyValuePair<ChunkInfo, List<AbstractChunk>> assetCollection in chunkCollections)
				{
					foreach (AbstractChunk chunk in assetCollection.Value)
					{
						((IProgress<float>)progress).Report(0.5f + i++ / (float)chunkCount / 2);
						((IProgress<string>)progressDescription).Report($"Writing file contents of \"{chunk.Name}\" to '{binaryFileTypeName}' file.");

						uint startOffset = (uint)(HeaderSize + tocBuffer.Length + assetStream.Position);
						chunk.StartOffset = startOffset;

						// Write start offset to TOC stream.
						Buffer.BlockCopy(BitConverter.GetBytes(startOffset), 0, tocBuffer, (int)startOffsetBytePositions[chunk], sizeof(uint));

						// Write asset data to asset stream.
						byte[] bytes = chunk.GetBuffer();
						assetStream.Write(bytes, 0, bytes.Length);
					}
				}
				assetBuffer = assetStream.ToArray();
			}

			// Create file.
			using FileStream fs = File.Create(outputPath);

			// Write file header.
			((IProgress<string>)progressDescription).Report($"Writing header buffer to '{binaryFileTypeName}' file.");
			fs.Write(BitConverter.GetBytes((uint)Magic1), 0, sizeof(uint));
			fs.Write(BitConverter.GetBytes((uint)Magic2), 0, sizeof(uint));
			fs.Write(BitConverter.GetBytes((uint)tocBuffer.Length), 0, sizeof(uint));

			// Write TOC buffer.
			((IProgress<string>)progressDescription).Report($"Writing TOC buffer to '{binaryFileTypeName}' file.");
			fs.Write(tocBuffer, 0, tocBuffer.Length);

			// Write asset buffer.
			((IProgress<string>)progressDescription).Report($"Writing asset buffer to '{binaryFileTypeName}' file.");
			fs.Write(assetBuffer, 0, assetBuffer.Length);
		}

		private Dictionary<ChunkInfo, List<AbstractChunk>> GetChunks(List<AbstractAsset> allAssets, Progress<float> progress, Progress<string> progressDescription, out int chunkCount)
		{
			chunkCount = 0;
			Dictionary<ChunkInfo, List<AbstractChunk>> assetCollections = new Dictionary<ChunkInfo, List<AbstractChunk>>();
			foreach (ChunkInfo chunkInfo in BinaryFileUtils.ChunkInfos.Where(c => BinaryFileType.HasFlagBothWays(c.BinaryFileType)))
			{
				StringBuilder loudness = new StringBuilder();

				AbstractAsset[] assets = allAssets.Where(a => a.ChunkTypeName == chunkInfo.Type.Name).ToArray();

				List<AbstractChunk> chunks = new List<AbstractChunk>();
				foreach (AbstractAsset asset in assets)
				{
					((IProgress<float>)progress).Report(chunks.Count / (float)allAssets.Count / 2);
					((IProgress<string>)progressDescription).Report($"Generating {chunkInfo.Type.Name.Replace("Chunk", "")} chunk \"{asset.AssetName}\".");

					if (asset is AudioAsset audioAsset)
						loudness.AppendLine($"{audioAsset.AssetName} = {audioAsset.Loudness.ToString("0.0")}");

					// Create chunk based on file buffer.
					byte[] fileBuffer;
					using (MemoryStream ms = new MemoryStream())
					{
						// TODO: Shaders have multiple files.

						//foreach (string assetFilePath in asset.EditorPath)
						//{
						//	byte[] fileContents = File.ReadAllBytes(assetFilePath);
						//	ms.Write(fileContents, 0, fileContents.Length);
						//}

						byte[] fileContents = File.ReadAllBytes(asset.EditorPath);
						ms.Write(fileContents, 0, fileContents.Length);
						fileBuffer = ms.ToArray();
					}
					AbstractChunk chunk = (AbstractChunk)Activator.CreateInstance(chunkInfo.Type, asset.AssetName, 0U/*Don't know start offset yet.*/, (uint)fileBuffer.Length, 0U);

					// Create header based on file buffer and chunk type.
					if (chunkInfo.Type == typeof(AbstractHeaderedChunk<AbstractHeader>))
					{
						AbstractHeaderedChunk<AbstractHeader> headeredChunk = (AbstractHeaderedChunk<AbstractHeader>)Activator.CreateInstance(chunkInfo.Type);

						byte[] headerBuffer = new byte[headeredChunk.Header.ByteCount];
						// TODO: Read fileBuffer and create headerBuffer.

						byte[] chunkBuffer = new byte[headerBuffer.Length + fileBuffer.Length];
						Buffer.BlockCopy(headerBuffer, 0, chunkBuffer, 0, headerBuffer.Length);
						Buffer.BlockCopy(fileBuffer, 0, chunkBuffer, headerBuffer.Length, fileBuffer.Length);

						chunk.SetBuffer(chunkBuffer);
					}
					else
					{
						chunk.SetBuffer(fileBuffer);
					}

					chunks.Add(chunk);
				}

				if (chunkInfo.Type == typeof(AudioChunk))
				{
					// Create loudness chunk.
					((IProgress<string>)progressDescription).Report("Generating Loudness chunk.");
					byte[] fileBuffer;
					using (MemoryStream ms = new MemoryStream())
					{
						byte[] fileContents = Encoding.Default.GetBytes(loudness.ToString());
						ms.Write(fileContents, 0, fileContents.Length);
						fileBuffer = ms.ToArray();
					}

					AbstractChunk loudnessChunk = (AbstractChunk)Activator.CreateInstance(chunkInfo.Type, "loudness", 0U/*Don't know start offset yet.*/, (uint)fileBuffer.Length, 0U);
					loudnessChunk.SetBuffer(fileBuffer);
					chunks.Add(loudnessChunk);
				}

				assetCollections[chunkInfo] = chunks;
				chunkCount += chunks.Count;
			}

			return assetCollections;
		}

		/// <summary>
		/// Extracts a compressed binary file into multiple asset files.
		/// </summary>
		/// <param name="inputPath">The binary file path.</param>
		/// <param name="outputPath">The path where the extracted asset files will be placed.</param>
		public override void Extract(string inputPath, string outputPath, Progress<float> progress, Progress<string> progressDescription)
		{
			// Read file contents.
			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			// Validate file.
			((IProgress<string>)progressDescription).Report("Validating file.");
			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			uint magic2FromFile = BitConverter.ToUInt32(sourceFileBytes, 4);
			if (magic1FromFile != Magic1 && magic2FromFile != Magic2)
				throw new Exception($"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {magic1FromFile} should be {Magic1}\nHeader value 2: {magic2FromFile} should be {Magic2}");

			// Read toc buffer.
			((IProgress<string>)progressDescription).Report("Reading TOC.");
			uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);
			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)tocSize);

			// Create chunks based on toc buffer.
			((IProgress<string>)progressDescription).Report("Creating chunks.");
			List<AbstractChunk> chunks = ReadChunks(tocBuffer);

			// Create folders and files based on chunks.
			((IProgress<string>)progressDescription).Report("Initializing extraction.");
			CreateFiles(outputPath, sourceFileBytes, chunks, progress, progressDescription);
		}

		private List<AbstractChunk> ReadChunks(byte[] tocBuffer)
		{
			List<AbstractChunk> chunks = new List<AbstractChunk>();

			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				ushort type = BitConverter.ToUInt16(tocBuffer, i);
				string name = ReadNullTerminatedString(tocBuffer, i + 2);

				i += name.Length + 1; // + 1 to include null terminator.
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				uint unknown = BitConverter.ToUInt32(tocBuffer, i + 10);
				i += 14;

				chunks.Add(Activator.CreateInstance(BinaryFileUtils.ChunkInfos.Where(c => c.BinaryTypes.Contains(type)).FirstOrDefault().Type, name, startOffset, size, unknown) as AbstractChunk);
			}

			return chunks;
		}

		private void CreateFiles(string outputPath, byte[] sourceFileBytes, IEnumerable<AbstractChunk> chunks, Progress<float> progress, Progress<string> progressDescription)
		{
			int chunksDone = 0;
			int totalChunks = chunks.Count();

			foreach (ChunkInfo chunkInfo in BinaryFileUtils.ChunkInfos.Where(c => BinaryFileType.HasFlagBothWays(c.BinaryFileType)))
				Directory.CreateDirectory(Path.Combine(outputPath, chunkInfo.FolderName));

			foreach (AbstractChunk chunk in chunks)
			{
				if (chunk.Size == 0)
					continue;

				ChunkInfo info = BinaryFileUtils.ChunkInfos.Where(c => c.Type == chunk.GetType()).FirstOrDefault();

				((IProgress<float>)progress).Report(chunksDone++ / (float)totalChunks);
				((IProgress<string>)progressDescription).Report($"Creating {info.Type.Name.Replace("Chunk", "")} file{(info.Type == typeof(ShaderChunk) ? "(s)" : "")} for chunk \"{chunk.Name}\".");

				byte[] buf = new byte[chunk.Size];
				Buffer.BlockCopy(sourceFileBytes, (int)chunk.StartOffset, buf, 0, (int)chunk.Size);

				chunk.SetBuffer(buf);

				foreach (FileResult fileResult in chunk.ToFileResult())
					File.WriteAllBytes(Path.Combine(outputPath, info.FolderName, $"{fileResult.Name}{(fileResult.Name == "loudness" && info.FileExtension == ".wav" ? ".ini" : info.FileExtension)}"), fileResult.Buffer);
			}
		}
	}
}