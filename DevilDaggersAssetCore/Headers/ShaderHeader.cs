namespace DevilDaggersAssetCore.Headers
{
	public class ShaderHeader : AbstractHeader
	{
		public override int ByteCount => 12;

		public uint NameLength { get; set; }
		public uint VertexSize { get; set; }
		public uint FragmentSize { get; set; }
	}
}