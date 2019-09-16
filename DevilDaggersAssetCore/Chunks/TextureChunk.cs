using DevilDaggersAssetCore.Headers;
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

		public override IEnumerable<FileResult> Extract()
		{
			using Bitmap bitmap = new Bitmap((int)Header.Width, (int)Header.Height, (int)Header.Width * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0));
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					Color pixel = bitmap.GetPixel(x, y);
					bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, pixel.B, pixel.G, pixel.R)); // Switch Blue and Red channels.
				}
			}

			yield return new FileResult(Name, GetBytes(bitmap));
		}

		private static byte[] GetBytes(Bitmap image)
		{
			MemoryStream memoryStream = new MemoryStream();

			// Create a new BitMap object to prevent "a generic GDI+ error" from being thrown.
			new Bitmap(image).Save(memoryStream, ImageFormat.Png);

			return memoryStream.ToArray();
		}
	}
}