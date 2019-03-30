using DevilDaggersAssetCore.Headers;

namespace DevilDaggersAssetCore.Chunks
{
	// TODO: Figure out how to encode/decode PNGs. (There is no working LodePNG for .NET unfortunately.)
	public class TextureChunk : AbstractHeaderedChunk<TextureHeader>
	{
		public TextureChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}
	}
}