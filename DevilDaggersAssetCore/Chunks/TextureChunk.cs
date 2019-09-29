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
		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Compress(string path)
		{
			Image image = Image.FromFile(path);

			byte mipmaps = (byte)(Math.Log(Math.Min(image.Width, image.Height), 2) + 1);

			byte[] headerBuffer = new byte[11]; // TODO: Get from TextureHeader.ByteCount but without creating an instance.
			System.Buffer.BlockCopy(BitConverter.GetBytes((ushort)16401), 0, headerBuffer, 0, sizeof(ushort));
			System.Buffer.BlockCopy(BitConverter.GetBytes(image.Width), 0, headerBuffer, 2, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes(image.Height), 0, headerBuffer, 6, sizeof(uint));
			System.Buffer.BlockCopy(new byte[] { mipmaps }, 0, headerBuffer, 10, sizeof(byte));
			Header = new TextureHeader(headerBuffer);

			int length = image.Width * image.Height * 4;

			int[] mipmapBufferSizes = new int[mipmaps];
			mipmapBufferSizes[0] = length;

			if (image.Width != image.Height)
			{
				int lengthMod = length;
				for (int i = 1; i < mipmaps; i++)
				{
					lengthMod /= 4;
					int mipmapSize = lengthMod;
					mipmapBufferSizes[i] = mipmapSize;
					length += mipmapSize;
				}
			}
			else
			{
				int lengthMod = image.Width;
				for (int i = 1; i < mipmaps; i++)
				{
					lengthMod /= 2;
					int mipmapSize = lengthMod * lengthMod * 4;
					mipmapBufferSizes[i] = mipmapSize;
					length += mipmapSize;
				}
			}

			Buffer = new byte[length];

			using (Bitmap bitmap = new Bitmap(image))
			{
				bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

				int mipmapOffset = 0;

				for (int i = 0; i < mipmaps - 1; i++)
				{
					int increment = (int)Math.Pow(2, i);
					int bufferPosition = 0;
					for (int x = 0; x < bitmap.Width; x += increment)
					{
						for (int y = 0; y < bitmap.Height; y += increment)
						{
							Color pixel = bitmap.GetPixel(x, y);

							// To prevent 240x240 textures from going out of bounds for some reason.
							if (bufferPosition >= mipmapBufferSizes[i])
								continue;

							System.Buffer.BlockCopy(new byte[] { pixel.R }, 0, Buffer, mipmapOffset + bufferPosition, sizeof(byte));
							System.Buffer.BlockCopy(new byte[] { pixel.G }, 0, Buffer, mipmapOffset + bufferPosition + 1, sizeof(byte));
							System.Buffer.BlockCopy(new byte[] { pixel.B }, 0, Buffer, mipmapOffset + bufferPosition + 2, sizeof(byte));
							System.Buffer.BlockCopy(new byte[] { pixel.A }, 0, Buffer, mipmapOffset + bufferPosition + 3, sizeof(byte));
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
			using Bitmap bitmap = new Bitmap((int)Header.Width, (int)Header.Height, (int)Header.Width * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0));
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					Color pixel = bitmap.GetPixel(x, y);
					bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, pixel.B, pixel.G, pixel.R)); // Switch Blue and Red channels (reverse rgb).
				}
			}

			using MemoryStream memoryStream = new MemoryStream();
			// Create a new BitMap object to prevent "a generic GDI+ error" from being thrown.
			new Bitmap(bitmap).Save(memoryStream, ImageFormat.Png);

			yield return new FileResult(Name, memoryStream.ToArray());
		}
	}
}