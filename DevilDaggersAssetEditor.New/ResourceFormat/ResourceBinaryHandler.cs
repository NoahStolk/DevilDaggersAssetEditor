using DevilDaggersAssetEditor.New.ResourceFormat.BinaryAssets;
using DevilDaggersAssetEditor.New.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.New.ResourceFormat
{
	public static class ResourceBinaryHandler
	{
		/// <summary>
		/// uint magic1, uint magic2, uint tocBufferSize = 12 bytes.
		/// </summary>
		public const int HeaderSize = 12;

		public static readonly ulong Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
		public static readonly ulong Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

		private static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
			=> a | b << 8 | c << 16 | d << 24;

		#region Construct binary

		/// <summary>
		/// Inserts multiple asset files into one binary file that can be read by Devil Daggers.
		/// </summary>
		/// <param name="fileAssets">The list of asset objects.</param>
		/// <param name="outputPath">The path where the binary file will be placed.</param>
		public static void ConstructBinary(List<ResourceFileAsset> fileAssets, string outputPath)
		{
			List<BinaryAsset> binaryAssets = ConvertFilesToBinary(fileAssets);

			// Create TOC stream.
			(byte[] tocBuffer, Dictionary<BinaryAsset, long> startOffsetBytePositions) = CreateTocStream(binaryAssets);

			// Create asset stream.
			byte[] assetBuffer = CreateAssetBuffer(binaryAssets, tocBuffer, startOffsetBytePositions);

			// Create file.
			byte[] binaryBytes = CreateBinary(tocBuffer, assetBuffer);

			using FileStream fs = File.Create(outputPath);
			fs.Write(binaryBytes, 0, binaryBytes.Length);
		}

		private static List<BinaryAsset> ConvertFilesToBinary(List<ResourceFileAsset> allAssets)
		{
			List<BinaryAsset> binaryAssets = new();
			foreach (ResourceFileAsset asset in allAssets)
			{
				BinaryAsset binaryAsset = asset.Type switch
				{
					ResourceAssetType.Audio => new AudioBinaryAsset(asset.AssetName),
					ResourceAssetType.Model => new ModelBinaryAsset(asset.AssetName),
					ResourceAssetType.ModelBinding => new ModelBindingBinaryAsset(asset.AssetName),
					ResourceAssetType.Shader => new ShaderBinaryAsset(asset.AssetName),
					ResourceAssetType.Texture => new TextureBinaryAsset(asset.AssetName),
					_ => throw ExceptionUtils.EnumNotImplemented(asset.Type),
				};

				if (asset.CurrentPath != null)
					binaryAsset.Construct(File.ReadAllBytes(asset.CurrentPath).ToImmutableArray());

				binaryAssets.Add(binaryAsset);
			}

			return binaryAssets;
		}

		public static (byte[] tocBuffer, Dictionary<BinaryAsset, long> startOffsetBytePositions) CreateTocStream(List<BinaryAsset> binaryAssets)
		{
			Dictionary<BinaryAsset, long> startOffsetBytePositions = new();
			using MemoryStream tocStream = new();
			foreach (BinaryAsset binaryAsset in binaryAssets)
			{
				// Write binary type.
				tocStream.Write(new[] { binaryAsset.Type }, 0, sizeof(byte));
				tocStream.Position++;

				// Write name.
				tocStream.Write(Encoding.Default.GetBytes(binaryAsset.AssetName), 0, binaryAsset.AssetName.Length);
				tocStream.Position++;

				// Skip writing start offsets for now, we'll write them once the TOC buffer size is defined.
				startOffsetBytePositions[binaryAsset] = tocStream.Position;
				tocStream.Position += sizeof(uint);

				// Write size.
				tocStream.Write(BitConverter.GetBytes(binaryAsset.Size), 0, sizeof(uint));

				// Skip the unknown value.
				tocStream.Position += sizeof(uint);
			}

			// Empty value between TOC and assets.
			tocStream.Write(BitConverter.GetBytes(0), 0, 2);

			return (tocStream.ToArray(), startOffsetBytePositions);
		}

		public static byte[] CreateAssetBuffer(List<BinaryAsset> binaryAssets, byte[] tocBuffer, Dictionary<BinaryAsset, long> startOffsetBytePositions)
		{
			byte[] assetBuffer;
			using MemoryStream assetStream = new();
			foreach (BinaryAsset binaryAsset in binaryAssets)
			{
				uint startOffset = (uint)(HeaderSize + tocBuffer.Length + assetStream.Position);
				binaryAsset.StartOffset = startOffset;

				// Write start offset to TOC stream.
				Buffer.BlockCopy(BitConverter.GetBytes(startOffset), 0, tocBuffer, (int)startOffsetBytePositions[binaryAsset], sizeof(uint));

				// Write asset data to asset stream.
				assetStream.Write(binaryAsset.Contents.ToArray(), 0, (int)binaryAsset.Size);
			}

			assetBuffer = assetStream.ToArray();

			return assetBuffer;
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

		#endregion Construct binary

		#region Extract binary

		/// <summary>
		/// Extracts a binary file into multiple asset files.
		/// </summary>
		/// <param name="inputPath">The binary file path.</param>
		/// <param name="outputPath">The path where the extracted asset files will be placed.</param>
		public static void ExtractBinary(string inputPath, string outputPath)
		{
			byte[] sourceFileBytes = File.ReadAllBytes(inputPath);

			if (!ValidateFile(sourceFileBytes))
			{
				// TODO: Show error:
				// $"Invalid file format. At least one of the two magic number values is incorrect:\n\nHeader value 1: {magic1FromFile} should be {Magic1}\nHeader value 2: {magic2FromFile} should be {Magic2}"
				return;
			}

			byte[] tocBuffer = ReadTocBuffer(sourceFileBytes);

			List<BinaryAsset> binaryAssets = ReadChunks(tocBuffer);

			CreateFiles(outputPath, sourceFileBytes, binaryAssets);
		}

		public static bool ValidateFile(byte[] sourceFileBytes)
		{
			uint magic1FromFile = BitConverter.ToUInt32(sourceFileBytes, 0);
			uint magic2FromFile = BitConverter.ToUInt32(sourceFileBytes, 4);
			return magic1FromFile == Magic1 && magic2FromFile == Magic2;
		}

		public static byte[] ReadTocBuffer(byte[] sourceFileBytes)
		{
			uint tocSize = BitConverter.ToUInt32(sourceFileBytes, 8);
			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(sourceFileBytes, 12, tocBuffer, 0, (int)tocSize);
			return tocBuffer;
		}

		public static List<BinaryAsset> ReadChunks(byte[] tocBuffer)
		{
			List<BinaryAsset> chunks = new();

			int i = 0;

			while (i < tocBuffer.Length - 14)
			{
				byte type = tocBuffer[i];
				string name = BinaryUtils.ReadNullTerminatedString(tocBuffer, i + 2);

				i += name.Length + 1; // + 1 to include null terminator.
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				i += 14;

				BinaryAsset? binaryAsset = type switch
				{
					0x01 => new ModelBinaryAsset(name) { StartOffset = startOffset, Size = size },
					0x02 => new TextureBinaryAsset(name) { StartOffset = startOffset, Size = size },
					0x10 => new ShaderBinaryAsset(name) { StartOffset = startOffset, Size = size },
					0x20 => new AudioBinaryAsset(name) { StartOffset = startOffset, Size = size },
					0x80 => new ModelBindingBinaryAsset(name) { StartOffset = startOffset, Size = size },
					_ => null,
				};

				if (binaryAsset != null)
					chunks.Add(binaryAsset);
			}

			return chunks;
		}

		private static void CreateFiles(string outputDirectory, byte[] sourceFileBytes, IEnumerable<BinaryAsset> binaryAssets)
		{
			foreach (BinaryAsset binaryAsset in binaryAssets.Where(ba => ba.Size != 0))
			{
				byte[] contents = new byte[binaryAsset.Size];
				Buffer.BlockCopy(sourceFileBytes, (int)binaryAsset.StartOffset, contents, 0, (int)binaryAsset.Size);

				binaryAsset.Contents = contents.ToImmutableArray();

				// TODO: Multiple files for shaders.
				foreach (FileResult fileResult in binaryAsset.Extract())
					File.WriteAllBytes(Path.Combine(outputDirectory, $"{fileResult.FileName}.{binaryAsset.FileExtension}"), fileResult.Contents.ToArray());
			}
		}

		#endregion Extract binary
	}
}