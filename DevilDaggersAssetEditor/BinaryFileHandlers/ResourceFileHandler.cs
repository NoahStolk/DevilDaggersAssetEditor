using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public class ResourceFileHandler : IBinaryFileHandler
	{
		/// <summary>
		/// uint magic1, uint magic2, uint tocBufferSize = 12 bytes.
		/// </summary>
		public const int HeaderSize = 12;

		public static readonly ulong Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
		public static readonly ulong Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

		public ResourceFileHandler(BinaryFileType binaryFileType)
		{
			if (binaryFileType == BinaryFileType.Particle)
				throw new NotSupportedException($"{nameof(BinaryFileType.Particle)} is unsupported by {nameof(ResourceFileHandler)}, use {nameof(ParticleFileHandler)} instead.");

			BinaryFileName = binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture);
		}

		public string BinaryFileName { get; }

		private static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d) => a | b << 8 | c << 16 | d << 24;

		#region Make binary

		/// <summary>
		/// Inserts multiple asset files into one binary file that can be read by Devil Daggers.
		/// </summary>
		/// <param name="allAssets">The list of asset objects.</param>
		/// <param name="outputPath">The path where the binary file will be placed.</param>
		/// <param name="progress">The progress wrapper to report progress to.</param>
		public void MakeBinary(List<AbstractAsset> allAssets, string outputPath, ProgressWrapper progress)
		{
			progress.Report($"Initializing '{BinaryFileName}' file creation.");

			allAssets = allAssets.Where(a => File.Exists(a.EditorPath)).ToList(); // TODO: Also check if FragmentShader file exists.

			progress.Report("Generating chunks based on asset list.");
			List<ResourceChunk> chunks = CreateChunksFromAssets(allAssets, progress);

			progress.Report("Generating TOC stream.");
			CreateTocStream(chunks, out byte[] tocBuffer, out Dictionary<ResourceChunk, long> startOffsetBytePositions);

			progress.Report("Generating asset stream.");
			byte[] assetBuffer = CreateAssetStream(chunks, tocBuffer, startOffsetBytePositions, progress);

			progress.Report($"Writing buffers to '{BinaryFileName}' file.");
			byte[] binaryBytes = CreateBinary(tocBuffer, assetBuffer);

			progress.Report($"Writing '{BinaryFileName}' file.");
			File.WriteAllBytes(outputPath, binaryBytes);
		}

		private static List<ResourceChunk> CreateChunksFromAssets(List<AbstractAsset> allAssets, ProgressWrapper progress)
		{
			StringBuilder loudness = new StringBuilder();

			List<ResourceChunk> chunks = new List<ResourceChunk>();
			foreach (AbstractAsset asset in allAssets)
			{
				progress.Report($"Generating {asset.AssetType} chunk \"{asset.AssetName}\".", chunks.Count / (float)allAssets.Count / 2);

				if (asset is AudioAsset audioAsset)
					loudness.AppendLine($"{audioAsset.AssetName} = {audioAsset.Loudness:0.0}");

				ResourceChunk chunk = asset.AssetType switch
				{
					AssetType.Model => new ModelChunk(asset.AssetName, 0, 0),
					AssetType.Shader => new ShaderChunk(asset.AssetName, 0, 0),
					AssetType.Texture => new TextureChunk(asset.AssetName, 0, 0),
					_ => new ResourceChunk(asset.AssetType, asset.AssetName, 0, 0),
				};
				chunk.MakeBinary(asset.EditorPath);

				chunks.Add(chunk);
			}

			if (loudness.Length != 0)
			{
				progress.Report("Generating Loudness chunk.");
				byte[] fileBuffer;
				using (MemoryStream ms = new MemoryStream())
				{
					byte[] fileContents = Encoding.Default.GetBytes(loudness.ToString());
					ms.Write(fileContents, 0, fileContents.Length);
					fileBuffer = ms.ToArray();
				}

				chunks.Add(new ResourceChunk(AssetType.Audio, "loudness", 0U, (uint)fileBuffer.Length) { Buffer = fileBuffer });
			}

			return chunks;
		}

		public static void CreateTocStream(List<ResourceChunk> chunks, out byte[] tocBuffer, out Dictionary<ResourceChunk, long> startOffsetBytePositions)
		{
			startOffsetBytePositions = new Dictionary<ResourceChunk, long>();
			using MemoryStream tocStream = new MemoryStream();
			foreach (ResourceChunk chunk in chunks)
			{
				// Write binary type.
				tocStream.Write(new[] { chunk.AssetType.GetBinaryType() }, 0, sizeof(byte));
				tocStream.Position++;

				// Write name.
				tocStream.Write(Encoding.Default.GetBytes(chunk.Name), 0, chunk.Name.Length);
				tocStream.Position++;

				// Write start offsets when TOC buffer size is defined.
				startOffsetBytePositions[chunk] = tocStream.Position;
				tocStream.Position += sizeof(uint);

				// Write size.
				tocStream.Write(BitConverter.GetBytes(chunk.Size), 0, sizeof(uint));

				// No reason to write unknown value.
				tocStream.Position += sizeof(uint);
			}

			tocStream.Write(BitConverter.GetBytes(0), 0, 2); // Empty value between TOC and assets.
			tocBuffer = tocStream.ToArray();
		}

		private byte[] CreateAssetStream(List<ResourceChunk> chunks, byte[] tocBuffer, Dictionary<ResourceChunk, long> startOffsetBytePositions, ProgressWrapper progress)
		{
			using MemoryStream assetStream = new MemoryStream();
			int i = 0;
			foreach (ResourceChunk chunk in chunks)
			{
				progress.Report($"Writing file contents of \"{chunk.Name}\" to '{BinaryFileName}' file.", 0.5f + i++ / (float)chunks.Count / 2);

				uint startOffset = (uint)(HeaderSize + tocBuffer.Length + assetStream.Position);
				chunk.StartOffset = startOffset;

				// Write start offset to TOC stream.
				Buffer.BlockCopy(BitConverter.GetBytes(startOffset), 0, tocBuffer, (int)startOffsetBytePositions[chunk], sizeof(uint));

				// Write asset data to asset stream.
				byte[] bytes = chunk.Buffer.ToArray();
				assetStream.Write(bytes, 0, bytes.Length);
			}

			return assetStream.ToArray();
		}

		public static byte[] CreateBinary(byte[] tocBuffer, byte[] assetBuffer)
		{
			using MemoryStream ms = new MemoryStream();

			// Write file header.
			ms.Write(BitConverter.GetBytes((uint)Magic1), 0, sizeof(uint));
			ms.Write(BitConverter.GetBytes((uint)Magic2), 0, sizeof(uint));
			ms.Write(BitConverter.GetBytes((uint)tocBuffer.Length), 0, sizeof(uint));

			// Write TOC buffer.
			ms.Write(tocBuffer, 0, tocBuffer.Length);

			// Write asset buffer.
			ms.Write(assetBuffer, 0, assetBuffer.Length);
			return ms.ToArray();
		}

		#endregion Make binary

		#region Extract binary

		/// <summary>
		/// Extracts a binary file into multiple asset files.
		/// </summary>
		/// <param name="inputPath">The binary file path.</param>
		/// <param name="outputPath">The path where the extracted asset files will be placed.</param>
		/// <param name="progress">The progress wrapper to report progress to.</param>
		public void ExtractBinary(string inputPath, string outputPath, ProgressWrapper progress)
		{
			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			progress.Report("Validating file.");
			ValidateFile(sourceFileBytes);

			progress.Report("Reading TOC buffer.");
			byte[] tocBuffer = ReadTocBuffer(sourceFileBytes);

			progress.Report("Creating chunks.");
			List<ResourceChunk> chunks = ReadChunks(tocBuffer);

			progress.Report("Initializing extraction.");
			CreateFiles(outputPath, sourceFileBytes, chunks, progress);
		}

		public void ValidateFile(byte[] sourceFileBytes)
		{
			// TODO: Show message instead of throwing exception.
			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			uint magic2FromFile = BitConverter.ToUInt32(sourceFileBytes, 4);
			if (magic1FromFile != Magic1 && magic2FromFile != Magic2)
				throw new($"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {magic1FromFile} should be {Magic1}\nHeader value 2: {magic2FromFile} should be {Magic2}");
		}

		public static byte[] ReadTocBuffer(byte[] sourceFileBytes)
		{
			uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);
			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)tocSize);
			return tocBuffer;
		}

		public static List<ResourceChunk> ReadChunks(byte[] tocBuffer)
		{
			List<ResourceChunk> chunks = new List<ResourceChunk>();

			int i = 0;

			// TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			while (i < tocBuffer.Length - 14)
			{
				byte type = tocBuffer[i];
				string name = BinaryUtils.ReadNullTerminatedString(tocBuffer, i + 2);

				i += name.Length + 1; // + 1 to include null terminator.
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				i += 14;

				AssetType? assetType = type.GetAssetType();
				if (assetType.HasValue)
				{
					ResourceChunk chunk = assetType switch
					{
						AssetType.Model => new ModelChunk(name, startOffset, size),
						AssetType.Shader => new ShaderChunk(name, startOffset, size),
						AssetType.Texture => new TextureChunk(name, startOffset, size),
						_ => new ResourceChunk(assetType.Value, name, startOffset, size),
					};
					chunks.Add(chunk);
				}
			}

			return chunks;
		}

		private static void CreateFiles(string outputPath, byte[] sourceFileBytes, IEnumerable<ResourceChunk> chunks, ProgressWrapper progress)
		{
			int chunksDone = 0;
			int totalChunks = chunks.Count();

			foreach (ResourceChunk chunk in chunks)
			{
				if (chunk.Size == 0) // Filter empty chunks (garbage in TOC buffers).
					continue;

				string fileExtension = chunk.AssetType.GetFileExtension();

				progress.Report(
					$"Creating {chunk.AssetType} file{(chunk.AssetType == AssetType.Shader ? "s" : string.Empty)} for chunk \"{chunk.Name}\".",
					chunksDone++ / (float)totalChunks);

				byte[] buffer = new byte[chunk.Size];
				Buffer.BlockCopy(sourceFileBytes, (int)chunk.StartOffset, buffer, 0, (int)chunk.Size);

				chunk.Buffer = buffer;

				foreach (FileResult fileResult in chunk.ExtractBinary())
				{
					string assetTypeDirectory = chunk.AssetType.GetFolderName();
					if (!Directory.Exists(assetTypeDirectory))
						Directory.CreateDirectory(Path.Combine(outputPath, assetTypeDirectory));
					File.WriteAllBytes(Path.Combine(outputPath, assetTypeDirectory, fileResult.Name + (fileResult.Name == "loudness" && fileExtension == ".wav" ? ".ini" : fileExtension)), fileResult.Buffer.ToArray());
				}
			}
		}

		#endregion Extract binary
	}
}
