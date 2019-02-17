namespace DevilDaggersAssetCore.Headers
{
	public class TextureHeader : AbstractHeader
	{
		public override int ByteCount => 11;

		public ushort Unknown { get; set; } // Color format?
		public uint Width { get; set; }
		public uint Height { get; set; }
		public byte Mipmaps { get; set; }
	}
}