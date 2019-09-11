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
			// Format32bppArgb
			using Bitmap bitmap = new Bitmap((int)Header.Width, (int)Header.Height, (int)Header.Width * 4, PixelFormat.Format32bppRgb, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0));
			yield return new FileResult(Name, GetBytes(bitmap));
		}

		public static byte[] GetBytes(Bitmap image)
		{
			MemoryStream memoryStream = new MemoryStream();
			new Bitmap(image).Save(memoryStream, ImageFormat.Png);
			return memoryStream.ToArray();
		}
	}
}