using DevilDaggersAssetCore.Headers;
using Newtonsoft.Json;
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
		private static readonly Dictionary<string, int> lengths;

		static TextureChunk()
		{
			using StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream("DevilDaggersAssetCore.Content.TextureLengths.json"));
			lengths = JsonConvert.DeserializeObject<Dictionary<string, int>>(sr.ReadToEnd());
		}

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
			System.Buffer.BlockCopy(new byte[] { (byte)(Math.Log(Math.Min(image.Width, image.Height), 2) + 1) }, 0, headerBuffer, 10, sizeof(byte));
			Header = new TextureHeader(headerBuffer);

			Buffer = new byte[lengths[Name]];

			using (Bitmap bitmap = new Bitmap(image))
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						Color pixel = bitmap.GetPixel(x, y);
						int pos = (x * image.Width / 4 + y) * 4;
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.B), 0, Buffer, pos, sizeof(byte));
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.G), 0, Buffer, pos + 1, sizeof(byte));
						System.Buffer.BlockCopy(BitConverter.GetBytes(pixel.R), 0, Buffer, pos + 2, sizeof(byte));
						//System.Buffer.BlockCopy(new byte[] { 255 }, 0, Buffer, pos + 3, sizeof(byte));
					}
				}
			}

			for (int i = 3; i < Buffer.Length; i += 4)
				System.Buffer.BlockCopy(new byte[] { 255 }, 0, Buffer, i, sizeof(byte));

			//Bitmap map = new Bitmap(image).Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb);

			//using (MemoryStream ms = new MemoryStream())
			//{
			//	new Bitmap(map).Save(ms, ImageFormat.Png);
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