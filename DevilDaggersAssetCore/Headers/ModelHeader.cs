namespace DevilDaggersAssetCore.Headers
{
	public class ModelHeader : AbstractHeader
	{
		public override int ByteCount => 10;

		public uint IndexCount { get; set; }
		public uint VertexCount { get; set; }
		public ushort Unknown { get; set; }
	}
}