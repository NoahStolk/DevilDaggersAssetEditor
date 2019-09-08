using DevilDaggersAssetCore.Headers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DevilDaggersAssetCore.Chunks
{
	public class TextureChunk : AbstractHeaderedChunk<TextureHeader>
	{
		// TODO: Build dll in x64.
		// TODO: Include dll file in the project itself when textures are working.
		private const string DllFilePath = @"..\..\..\LodePNG\Debug\LodePNG.dll";

		[DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall, ExactSpelling = false, CharSet = CharSet.Auto)]
		private extern static uint lodepng_encode32(out byte[] outchars, out uint outsize, byte[] image, uint w, uint h);

		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}

#if LODE_PNG
		public override IEnumerable<FileResult> Extract()
		{
			// TODO: Crashes with no errors whatsoever... Probably wrong signature.
			lodepng_encode32(out byte[] newBytes, out _, Buffer, Header.Width, Header.Height);

			yield return new FileResult(Name, newBytes);
		}
#endif
	}
}