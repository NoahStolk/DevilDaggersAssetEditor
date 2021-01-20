using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.User;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	public class TextureChunk : ResourceChunk
	{
		private readonly PngEncoder _pngEncoder = new();

		public TextureChunk(string name, uint startOffset, uint size)
			: base(AssetType.Texture, name, startOffset, size)
		{
		}

		public override void MakeBinary(string path)
		{
			using Image<Rgba32> image = Image.Load<Rgba32>(path);
			int maxDimension = Math.Max(image.Width, image.Height);
			int newWidth = image.Width;
			int newHeight = image.Height;
			if (AssetHandler.Instance.DdTexturesAssets.Find(t => t.AssetName == Name)?.IsModelTexture == true)
			{
				while (maxDimension > UserHandler.Instance.Settings.TextureSizeLimit)
				{
					newWidth /= 2;
					newHeight /= 2;
					maxDimension /= 2;
				}
			}

			using Image<Rgba32> resizedImage = ResizeImage(image, Math.Max(1, newWidth), Math.Max(1, newHeight));

			byte mipmapCount = GetMipmapCountFromImage(resizedImage);
			GetBufferSizes(resizedImage.Width, resizedImage.Height, mipmapCount, out int pixelBufferLength, out int[] mipmapBufferSizes);

			Buffer = new byte[11 + pixelBufferLength];
			Buf.BlockCopy(BitConverter.GetBytes((ushort)16401), 0, Buffer, 0, sizeof(ushort));
			Buf.BlockCopy(BitConverter.GetBytes(resizedImage.Width), 0, Buffer, 2, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(resizedImage.Height), 0, Buffer, 6, sizeof(uint));
			Buffer[10] = mipmapCount;

			int mipmapWidth = resizedImage.Width;
			int mipmapHeight = resizedImage.Height;
			int mipmapBufferOffset = 0;
			for (int i = 0; i < mipmapCount; i++)
			{
				using Image<Rgba32> bitmap = ResizeImage(resizedImage, mipmapWidth, mipmapHeight);

				if (image.TryGetSinglePixelSpan(out Span<Rgba32> colors))
				{
					int bufferPosition = 0;

					for (int x = 0; x < bitmap.Width; x++)
					{
						for (int y = 0; y < bitmap.Height; y++)
						{
							// To prevent 240x240 textures from going out of bounds.
							if (bufferPosition >= mipmapBufferSizes[i])
								continue;

							Rgba32 pixel = colors[x * bitmap.Height + y];
							Buffer[11 + mipmapBufferOffset + bufferPosition++] = pixel.R;
							Buffer[11 + mipmapBufferOffset + bufferPosition++] = pixel.G;
							Buffer[11 + mipmapBufferOffset + bufferPosition++] = pixel.B;
							Buffer[11 + mipmapBufferOffset + bufferPosition++] = pixel.A;
						}
					}

					mipmapBufferOffset += mipmapBufferSizes[i];
					mipmapWidth /= 2;
					mipmapHeight /= 2;
				}
				else
				{
					throw new("Image is corrupt.");
				}
			}

			Size = (uint)Buffer.Length;
		}

		public override IEnumerable<FileResult> ExtractBinary()
		{
			uint width = BitConverter.ToUInt32(Buffer, 2);
			uint height = BitConverter.ToUInt32(Buffer, 6);
			byte mipmapCount = Buffer[10];

			GetBufferSizes((int)width, (int)height, mipmapCount, out _, out int[] _);

			using Image<Rgba32> image = new((int)width, (int)height);
			for (int y = 0; y < image.Height; y++)
			{
				Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y);
				for (int x = 0; x < image.Width; x++)
				{
					int index = 11 + (x * image.Height + y) * 4;
					pixelRowSpan[x] = new(Buffer[index], Buffer[index + 1], Buffer[index + 2], Buffer[index + 3]);
				}
			}

			using MemoryStream memoryStream = new();
			image.Save(memoryStream, _pngEncoder);
			yield return new FileResult(Name, memoryStream.ToArray());
		}

		private static byte GetMipmapCountFromImage(Image image)
			=> TextureAsset.GetMipmapCount(image.Width, image.Height);

		private static void GetBufferSizes(int width, int height, byte mipmapCount, out int totalBufferLength, out int[] mipmapBufferSizes)
		{
			totalBufferLength = width * height * 4;
			mipmapBufferSizes = new int[mipmapCount];
			mipmapBufferSizes[0] = totalBufferLength;

			if (width != height)
			{
				int lengthMod = totalBufferLength;
				for (int i = 1; i < mipmapCount; i++)
				{
					lengthMod /= 4;

					mipmapBufferSizes[i] = lengthMod;
					totalBufferLength += lengthMod;
				}
			}
			else
			{
				int lengthMod = width;
				for (int i = 1; i < mipmapCount; i++)
				{
					lengthMod /= 2;

					int mipmapSize = lengthMod * lengthMod * 4;
					mipmapBufferSizes[i] = mipmapSize;
					totalBufferLength += mipmapSize;
				}
			}
		}

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		private static Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height)
			=> image.Clone(x => x.Resize(width, height, KnownResamplers.Lanczos3));
	}
}
