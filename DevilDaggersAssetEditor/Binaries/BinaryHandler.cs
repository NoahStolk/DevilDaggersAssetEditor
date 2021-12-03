using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Binaries.Chunks;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Progress;
using DevilDaggersCore.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.Binaries;

public static class BinaryHandler
{
	/// <summary>
	/// The header consists of three 32-bit integers (Magic1, Magic2, and TocBufferSize), which is 12 bytes.
	/// </summary>
	public const int HeaderSize = 12;

	public static readonly ulong Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
	public static readonly ulong Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

	private static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
		=> a | b << 8 | c << 16 | d << 24;

	#region Make binary

	/// <summary>
	/// Inserts multiple asset files into one binary file that can be read by Devil Daggers.
	/// </summary>
	/// <param name="allAssets">The list of asset objects.</param>
	/// <param name="outputPath">The path where the binary file will be placed.</param>
	/// <param name="progress">The progress wrapper to report progress to.</param>
	public static void MakeBinary(List<AbstractAsset> allAssets, string outputPath, ProgressWrapper progress)
	{
		progress.Report("Initializing file creation.");

		allAssets = allAssets.Where(a => File.Exists(a.EditorPath) && (a is not ShaderAsset sa || File.Exists(sa.EditorPathFragmentShader))).ToList();

		progress.Report("Generating chunks based on asset list.");
		List<Chunk> chunks = CreateChunksFromAssets(allAssets, progress);

		progress.Report("Generating TOC stream.");
		CreateTocStream(chunks, out byte[] tocBuffer, out Dictionary<Chunk, long> startOffsetBytePositions);

		progress.Report("Generating asset stream.");
		byte[] assetBuffer = CreateAssetStream(chunks, tocBuffer, startOffsetBytePositions, progress);

		progress.Report("Writing buffers to file.");
		byte[] binaryBytes = CreateBinary(tocBuffer, assetBuffer);

		progress.Report("Writing file.");
		File.WriteAllBytes(outputPath, binaryBytes);
	}

	private static List<Chunk> CreateChunksFromAssets(List<AbstractAsset> allAssets, ProgressWrapper progress)
	{
		Dictionary<string, float> loudnessValues = new();

		List<Chunk> chunks = new();
		foreach (AbstractAsset asset in allAssets)
		{
			progress.Report($"Generating {asset.AssetType} chunk \"{asset.AssetName}\".", chunks.Count / (float)allAssets.Count / 2);

			if (asset is AudioAsset audioAsset)
				loudnessValues.Add(audioAsset.AssetName, audioAsset.Loudness);

			Chunk chunk = asset.AssetType switch
			{
				AssetType.Model => new ModelChunk(asset.AssetName, 0, 0),
				AssetType.Shader => new ShaderChunk(asset.AssetName, 0, 0),
				AssetType.Texture => new TextureChunk(asset.AssetName, 0, 0),
				_ => new Chunk(asset.AssetType, asset.AssetName, 0, 0),
			};
			chunk.MakeBinary(asset.EditorPath);

			chunks.Add(chunk);
		}

		// If any audio asset is included in this list, we need to create a loudness chunk as well.
		if (loudnessValues.Count > 0)
		{
			// Any missing audio will need to have its default loudness included, otherwise the game will play those with loudness 1.0.
			foreach (AudioAsset audioAsset in AssetContainer.Instance.AudioAudioAssets)
			{
				// Only add it to the list if it is present in the default loudness file, otherwise the game will detect prohibited mods.
				if (audioAsset.PresentInDefaultLoudness && !loudnessValues.ContainsKey(audioAsset.AssetName))
					loudnessValues.Add(audioAsset.AssetName, audioAsset.DefaultLoudness);
			}

			StringBuilder loudness = new();
			foreach (KeyValuePair<string, float> kvp in loudnessValues)
				loudness.Append(kvp.Key).Append(" = ").AppendFormat("{0:0.0}", kvp.Value).AppendLine();

			progress.Report("Generating Loudness chunk.");
			byte[] fileBuffer;
			using (MemoryStream ms = new())
			{
				byte[] fileContents = Encoding.Default.GetBytes(loudness.ToString());
				ms.Write(fileContents, 0, fileContents.Length);
				fileBuffer = ms.ToArray();
			}

			chunks.Add(new(AssetType.Audio, "loudness", 0U, (uint)fileBuffer.Length) { Buffer = fileBuffer });
		}

		return chunks;
	}

	public static void CreateTocStream(List<Chunk> chunks, out byte[] tocBuffer, out Dictionary<Chunk, long> startOffsetBytePositions)
	{
		startOffsetBytePositions = new();
		using MemoryStream tocStream = new();
		foreach (Chunk chunk in chunks)
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

	public static byte[] CreateAssetStream(List<Chunk> chunks, byte[] tocBuffer, Dictionary<Chunk, long> startOffsetBytePositions, ProgressWrapper? progress)
	{
		using MemoryStream assetStream = new();
		int i = 0;
		foreach (Chunk chunk in chunks)
		{
			progress?.Report($"Writing file contents of \"{chunk.Name}\" to file.", 0.5f + i++ / (float)chunks.Count / 2);

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
		using MemoryStream ms = new();

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
	public static string? ExtractBinary(string inputPath, string outputPath, ProgressWrapper progress)
	{
		byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

		progress.Report("Validating file.");
		if (!IsValidFile(sourceFileBytes))
			return "Invalid file format. Make sure to open one of the following binary files: audio, core, dd";

		progress.Report("Reading TOC buffer.");
		byte[] tocBuffer = ReadTocBuffer(sourceFileBytes);

		progress.Report("Creating chunks.");
		List<Chunk> chunks = ReadChunks(tocBuffer);

		progress.Report("Initializing extraction.");
		CreateFiles(outputPath, sourceFileBytes, chunks, progress);

		return null;
	}

	public static bool IsValidFile(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		fs.Seek(0, SeekOrigin.Begin);

		byte[] fileHeader = new byte[12];
		int bytesRead = fs.Read(fileHeader, 0, fileHeader.Length);
		if (bytesRead < fileHeader.Length)
			return false;

		uint magic1FromFile = BitConverter.ToUInt32(fileHeader, 0);
		uint magic2FromFile = BitConverter.ToUInt32(fileHeader, 4);
		if (magic1FromFile != Magic1 || magic2FromFile != Magic2)
			return false;

		uint tocSize = BitConverter.ToUInt32(fileHeader, 8);
		return tocSize <= fs.Length - 12;
	}

	public static bool IsValidFile(byte[] sourceFileBytes)
	{
		if (sourceFileBytes.Length < 12)
			return false;

		uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
		uint magic2FromFile = BitConverter.ToUInt32(sourceFileBytes, 4);
		if (magic1FromFile != Magic1 || magic2FromFile != Magic2)
			return false;

		uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);
		return tocSize <= sourceFileBytes.Length - 12;
	}

	public static byte[] ReadTocBuffer(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		fs.Seek(8, SeekOrigin.Begin);

		byte[] tocSizeBytes = new byte[sizeof(uint)];
		fs.Read(tocSizeBytes, 0, sizeof(uint));
		uint tocSize = BitConverter.ToUInt32(tocSizeBytes);

		byte[] tocBuffer = new byte[tocSize];
		fs.Read(tocBuffer, 0, (int)tocSize);
		return tocBuffer;
	}

	public static byte[] ReadTocBuffer(byte[] sourceFileBytes)
	{
		uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);
		byte[] tocBuffer = new byte[tocSize];
		Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)tocSize);
		return tocBuffer;
	}

	public static List<Chunk> ReadChunks(byte[] tocBuffer)
	{
		List<Chunk> chunks = new();

		int i = 0;
		while (i < tocBuffer.Length - 14)
		{
			byte type = tocBuffer[i];
			string name = ReadNullTerminatedString(tocBuffer, i + 2);

			i += name.Length + 1; // + 1 to include null terminator.
			uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
			uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
			i += 14;

			AssetType? assetType = type.GetAssetType();
			if (!assetType.HasValue)
				continue;

			Chunk chunk = assetType switch
			{
				AssetType.Model => new ModelChunk(name, startOffset, size),
				AssetType.Shader => new ShaderChunk(name, startOffset, size),
				AssetType.Texture => new TextureChunk(name, startOffset, size),
				_ => new Chunk(assetType.Value, name, startOffset, size),
			};
			chunks.Add(chunk);
		}

		return chunks;

		static string ReadNullTerminatedString(byte[] buffer, int offset)
		{
			StringBuilder sb = new();
			for (int i = offset; i < buffer.Length; i++)
			{
				char c = (char)buffer[i];
				if (c == '\0')
					return sb.ToString();
				sb.Append(c);
			}

			throw new($"Null terminator not observed in buffer with length {buffer.Length} starting from offset {offset}.");
		}
	}

	private static void CreateFiles(string outputPath, byte[] sourceFileBytes, IEnumerable<Chunk> chunks, ProgressWrapper progress)
	{
		int chunksDone = 0;
		int totalChunks = chunks.Count();

		foreach (Chunk chunk in chunks)
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

				if (fileResult.Name == "loudness" && fileExtension == ".wav")
					fileExtension = ".ini";

				File.WriteAllBytes(Path.Combine(outputPath, assetTypeDirectory, fileResult.Name + fileExtension), fileResult.Buffer.ToArray());
			}
		}
	}

	#endregion Extract binary
}