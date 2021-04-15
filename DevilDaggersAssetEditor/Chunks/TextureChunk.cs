using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.User;
using DevilDaggersCore.Mods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Buf = System.Buffer;

namespace DevilDaggersAssetEditor.Chunks
{
	public class TextureChunk : Chunk
	{
		public TextureChunk(string name, uint startOffset, uint size)
			: base(AssetType.Texture, name, startOffset, size)
		{
		}

		public override int HeaderSize => 11;

		public override void MakeBinary(string path)
		{
			using Image image = Image.FromFile(path);
			int maxDimension = Math.Max(image.Width, image.Height);
			int newWidth = image.Width;
			int newHeight = image.Height;
			if (AssetContainer.Instance.DdTexturesAssets.Find(t => t.AssetName == Name)?.IsModelTexture == true)
			{
				while (maxDimension > UserHandler.Instance.Settings.TextureSizeLimit)
				{
					newWidth /= 2;
					newHeight /= 2;
					maxDimension /= 2;
				}
			}

			using Bitmap resizedImage = ResizeImage(image, Math.Max(1, newWidth), Math.Max(1, newHeight));

			byte mipmapCount = GetMipmapCountFromImage(resizedImage);
			GetBufferSizes(resizedImage.Width, resizedImage.Height, mipmapCount, out int pixelBufferLength, out int[] mipmapBufferSizes);

			Buffer = new byte[HeaderSize + pixelBufferLength];
			Buf.BlockCopy(BitConverter.GetBytes((ushort)16401), 0, Buffer, 0, sizeof(ushort));
			Buf.BlockCopy(BitConverter.GetBytes(resizedImage.Width), 0, Buffer, 2, sizeof(uint));
			Buf.BlockCopy(BitConverter.GetBytes(resizedImage.Height), 0, Buffer, 6, sizeof(uint));
			Buffer[10] = mipmapCount;

			int mipmapWidth = resizedImage.Width;
			int mipmapHeight = resizedImage.Height;
			int mipmapBufferOffset = 0;
			for (int i = 0; i < mipmapCount; i++)
			{
				using Bitmap bitmap = ResizeImage(resizedImage, mipmapWidth, mipmapHeight);
				bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

				int bufferPosition = 0;

				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						Color pixel = bitmap.GetPixel(x, y);

						// To prevent 240x240 textures from going out of bounds.
						if (bufferPosition >= mipmapBufferSizes[i])
							continue;

						Buffer[HeaderSize + mipmapBufferOffset + bufferPosition++] = pixel.R;
						Buffer[HeaderSize + mipmapBufferOffset + bufferPosition++] = pixel.G;
						Buffer[HeaderSize + mipmapBufferOffset + bufferPosition++] = pixel.B;
						Buffer[HeaderSize + mipmapBufferOffset + bufferPosition++] = pixel.A;
					}
				}

				mipmapBufferOffset += mipmapBufferSizes[i];
				mipmapWidth /= 2;
				mipmapHeight /= 2;
			}

			Size = (uint)Buffer.Length;
		}

		public override IEnumerable<FileResult> ExtractBinary()
		{
			uint width = BitConverter.ToUInt32(Buffer, 2);
			uint height = BitConverter.ToUInt32(Buffer, 6);
			byte mipmapCount = Buffer[10];

			GetBufferSizes((int)width, (int)height, mipmapCount, out _, out int[] _);

			IntPtr intPtr = Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, HeaderSize);
			using Bitmap bitmap = new((int)width, (int)height, (int)width * 4, PixelFormat.Format32bppArgb, intPtr);
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					Color pixel = bitmap.GetPixel(x, y);
					bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, pixel.B, pixel.G, pixel.R)); // Switch Blue and Red channels (reverse RGBA).
				}
			}

			using MemoryStream memoryStream = new();

			// Create a new BitMap object to prevent "a generic GDI+ error" from being thrown.
			new Bitmap(bitmap).Save(memoryStream, ImageFormat.Png);

			yield return new(Name, memoryStream.ToArray());
		}

#if EXTRACT_MIPMAPS
		public override IEnumerable<FileResult> ExtractBinary()
		{
			uint width = BitConverter.ToUInt32(Buffer, 2);
			uint height = BitConverter.ToUInt32(Buffer, 6);
			byte mipmapCount = Buffer[10];

			GetBufferSizes((int)width, (int)height, mipmapCount, out _, out int[] mipmapBufferSizes);

			int mipmapOffset = 0;
			for (int i = 0; i < (_extractMipmaps ? mipmapCount : 1); i++)
			{
				int mipmapSizeDivisor = (int)Math.Pow(2, i);
				int bitmapWidth = (int)width / mipmapSizeDivisor;
				IntPtr intPtr = Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, mipmapOffset + HeaderSize);
				using Bitmap bitmap = new Bitmap(bitmapWidth, (int)height / mipmapSizeDivisor, bitmapWidth * 4, PixelFormat.Format32bppArgb, intPtr);
				bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						Color pixel = bitmap.GetPixel(x, y);
						bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, pixel.B, pixel.G, pixel.R)); // Switch Blue and Red channels (reverse RGBA).
					}
				}

				mipmapOffset += mipmapBufferSizes[i];

				using MemoryStream memoryStream = new MemoryStream();

				// Create a new BitMap object to prevent "a generic GDI+ error" from being thrown.
				new Bitmap(bitmap).Save(memoryStream, ImageFormat.Png);

				yield return new(Name + (_extractMipmaps ? $"_{bitmap.Width}x{bitmap.Height}" : string.Empty), memoryStream.ToArray());
			}
		}
#endif

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
		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			Rectangle destRect = new(0, 0, width, height);
			Bitmap destImage = new(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using ImageAttributes wrapMode = new();
				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
			}

			return destImage;
		}
	}
}
