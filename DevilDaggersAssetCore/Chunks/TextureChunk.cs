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
		private readonly List<int> bufferLengths = new List<int>
		{
			1398100,
			349524,
			307136,
			87380,
			87376,
			43688,
			21844,
			21824,
			5460,
			2728,
			1364
		};

		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

		public override void Compress(string path)
		{
			Image image = Image.FromFile(path);

			byte[] headerBuffer = new byte[11]; // TODO: Get from TextureHeader.ByteCount but without creating an instance.
			System.Buffer.BlockCopy(BitConverter.GetBytes(image.Width), 0, headerBuffer, 2, sizeof(uint));
			System.Buffer.BlockCopy(BitConverter.GetBytes(image.Height), 0, headerBuffer, 6, sizeof(uint));
			Header = new TextureHeader(headerBuffer);

			int lengthGuess = (int)Math.Floor(image.Width * image.Height * (16 / 3f) / 4) * 4;
			if (!bufferLengths.Contains(lengthGuess))
			{
				int currDiff = 100000;
				int newLength = 999999999;
				foreach (int length in bufferLengths)
				{
					if (Math.Abs(length - lengthGuess) < currDiff)
					{
						currDiff = Math.Abs(length - lengthGuess);
						newLength = length;
					}
				}
				lengthGuess = newLength;
			}
			Buffer = new byte[lengthGuess];

			using (Bitmap bitmap = new Bitmap(image))
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						Color pixel = bitmap.GetPixel(x, y);
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.A), 0, Buffer, x * y * 4, sizeof(byte));
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.B), 0, Buffer, x * y * 4 + 1, sizeof(byte));
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.G), 0, Buffer, x * y * 4 + 2, sizeof(byte));
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.R), 0, Buffer, x * y * 4 + 3, sizeof(byte));
					}
				}
			}

			//using (MemoryStream ms = new MemoryStream())
			//{
			//	new Bitmap(image).Save(ms, PixelFormat.Format32bppArgb);
			//	Buffer = ms.ToArray();
			//}

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

			MemoryStream memoryStream = new MemoryStream();

			// Create a new BitMap object to prevent "a generic GDI+ error" from being thrown.
			new Bitmap(bitmap).Save(memoryStream, ImageFormat.Png);

			yield return new FileResult(Name, memoryStream.ToArray());
		}
	}
}