using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Buf = System.Buffer;

namespace DevilDaggersAssetCore.Chunks
{
	public class TextureChunk : AbstractHeaderedChunk<TextureHeader>
	{
		private static readonly bool extractMipmaps = false;

		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Compress(string path)
		{
			using Image image = Image.FromFile(path);
			int maxDimension = Math.Max(image.Width, image.Height);
			int newWidth = image.Width;
			int newHeight = image.Height;
			while (maxDimension > 512/*settings.TextureSizeLimit*/)
			{
				newWidth /= 2;
				newHeight /= 2;
				maxDimension /= 2;
			}

			using Bitmap resizedImage = ResizeImage(image, Math.Max(1, newWidth), Math.Max(1, newHeight));

			byte[] headerBuffer = new byte[BinaryFileUtils.TextureHeaderByteCount];
			Buf.BlockCopy(BitConverter.GetBytes((ushort)16401), 0, headerBuffer, 0, sizeof(ushort));
			Buf.BlockCopy(BitConverter.GetBytes(resizedImage.Width), 0, headerBuffer, 2, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(resizedImage.Height), 0, headerBuffer, 6, sizeof(uint));
			headerBuffer[10] = GetMipmapCountFromImage(resizedImage);
			Header = new TextureHeader(headerBuffer);

			GetBufferSizes(Header, out int totalBufferLength, out int[] mipmapBufferSizes);

			Buffer = new byte[totalBufferLength];
			int mipmapWidth = resizedImage.Width;
			int mipmapHeight = resizedImage.Height;
			int mipmapBufferOffset = 0;
			for (int i = 0; i < Header.MipmapCount; i++)
			{
				using Bitmap bitmap = ResizeImage(resizedImage, mipmapWidth, mipmapHeight);
				bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

				int increment = (int)Math.Pow(2, i);
				int bufferPosition = 0;

				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						Color pixel = bitmap.GetPixel(x, y);

						// To prevent 240x240 textures from going out of bounds.
						if (bufferPosition >= mipmapBufferSizes[i])
							continue;

						Buffer[mipmapBufferOffset + bufferPosition++] = pixel.R;
						Buffer[mipmapBufferOffset + bufferPosition++] = pixel.G;
						Buffer[mipmapBufferOffset + bufferPosition++] = pixel.B;
						Buffer[mipmapBufferOffset + bufferPosition++] = pixel.A;
					}
				}

				mipmapBufferOffset += mipmapBufferSizes[i];
				mipmapWidth /= 2;
				mipmapHeight /= 2;
			}

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;
		}

		public override IEnumerable<FileResult> Extract()
		{
			GetBufferSizes(Header, out _, out int[] mipmapBufferSizes);

			int mipmapOffset = 0;
			for (int i = 0; i < (extractMipmaps ? Header.MipmapCount : 1); i++)
			{
				int mipmapSizeDivisor = (int)Math.Pow(2, i);
				IntPtr intPtr = Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, mipmapOffset);
				using Bitmap bitmap = new Bitmap((int)Header.Width / mipmapSizeDivisor, (int)Header.Height / mipmapSizeDivisor, (int)Header.Width / mipmapSizeDivisor * 4, PixelFormat.Format32bppArgb, intPtr);
				bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						Color pixel = bitmap.GetPixel(x, y);
						bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, pixel.B, pixel.G, pixel.R)); // Switch Blue and Red channels (reverse rgb).
					}
				}
				mipmapOffset += mipmapBufferSizes[i];

				using MemoryStream memoryStream = new MemoryStream();
				// Create a new BitMap object to prevent "a generic GDI+ error" from being thrown.
				new Bitmap(bitmap).Save(memoryStream, ImageFormat.Png);

				yield return new FileResult($"{Name}{(extractMipmaps ? $"_{bitmap.Width}x{bitmap.Height}" : "")}", memoryStream.ToArray());
			}
		}

		private static byte GetMipmapCountFromImage(Image image) => TextureAsset.GetMipmapCount(image.Width, image.Height);

		private static void GetBufferSizes(TextureHeader header, out int totalBufferLength, out int[] mipmapBufferSizes)
		{
			totalBufferLength = (int)header.Width * (int)header.Height * 4;
			mipmapBufferSizes = new int[header.MipmapCount];
			mipmapBufferSizes[0] = totalBufferLength;

			if (header.Width != header.Height)
			{
				int lengthMod = totalBufferLength;
				for (int i = 1; i < header.MipmapCount; i++)
				{
					lengthMod /= 4;

					mipmapBufferSizes[i] = lengthMod;
					totalBufferLength += lengthMod;
				}
			}
			else
			{
				int lengthMod = (int)header.Width;
				for (int i = 1; i < header.MipmapCount; i++)
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
		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			Rectangle destRect = new Rectangle(0, 0, width, height);
			Bitmap destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using ImageAttributes wrapMode = new ImageAttributes();
				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
			}

			return destImage;
		}
	}
}