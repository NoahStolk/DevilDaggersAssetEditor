using DevilDaggersAssetCore.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace DevilDaggersAssetCore.Chunks
{
	public class TextureChunk : AbstractHeaderedChunk<TextureHeader>
	{
		private static readonly bool ExtractMipmaps = false;

		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Compress(string path)
		{
			Image image = Image.FromFile(path);

			byte[] headerBuffer = new byte[11]; // TODO: Get from TextureHeader.ByteCount but without creating an instance.
			System.Buffer.BlockCopy(BitConverter.GetBytes((ushort)16401), 0, headerBuffer, 0, sizeof(ushort));
			System.Buffer.BlockCopy(BitConverter.GetBytes(image.Width), 0, headerBuffer, 2, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes(image.Height), 0, headerBuffer, 6, sizeof(uint));
			headerBuffer[10] = GetMipmapCountFromImage(image);
			Header = new TextureHeader(headerBuffer);

			GetBufferSizes(Header, out int totalBufferLength, out int[] mipmapBufferSizes);

			Buffer = new byte[totalBufferLength];

			using (Bitmap bitmap = new Bitmap(image))
			{
				bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

				int mipmapOffset = 0;
				for (int i = 0; i < Header.MipmapCount - 1; i++)
				{
					int increment = (int)Math.Pow(2, i);
					int bufferPosition = 0;

					// TODO: Figure out how mipmap blurring works to accurately reproduce this when recompressing assets. Now it's just skipping pixels which works but doesn't give the same exact result.
					for (int x = 0; x < bitmap.Width; x += increment)
					{
						for (int y = 0; y < bitmap.Height; y += increment)
						{
							Color pixel = bitmap.GetPixel(x, y);

							// To prevent 240x240 textures from going out of bounds for some reason.
							if (bufferPosition >= mipmapBufferSizes[i])
								continue;

							Buffer[mipmapOffset + bufferPosition] = pixel.R;
							Buffer[mipmapOffset + bufferPosition + 1] = pixel.G;
							Buffer[mipmapOffset + bufferPosition + 2] = pixel.B;
							Buffer[mipmapOffset + bufferPosition + 3] = pixel.A;
							bufferPosition += 4;
						}
					}
					mipmapOffset += mipmapBufferSizes[i];
				}
			}

			Size = (uint)Buffer.Length + (uint)Header.Buffer.Length;
		}

		public override IEnumerable<FileResult> Extract()
		{
			GetBufferSizes(Header, out int totalBufferLength, out int[] mipmapBufferSizes);

			int mipmapOffset = 0;
			for (int i = 0; i < (ExtractMipmaps ? Header.MipmapCount : 1); i++)
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

				yield return new FileResult($"{Name}{(ExtractMipmaps ? $"_{bitmap.Width}x{bitmap.Height}" : "")}", memoryStream.ToArray());
			}
		}

		private static byte GetMipmapCountFromImage(Image image)
		{
			return (byte)(Math.Log(Math.Min(image.Width, image.Height), 2) + 1);
		}

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
					int mipmapSize = lengthMod;
					mipmapBufferSizes[i] = mipmapSize;
					totalBufferLength += mipmapSize;
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
	}
}