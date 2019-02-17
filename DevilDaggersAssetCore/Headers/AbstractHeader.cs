namespace DevilDaggersAssetCore.Headers
{
	public abstract class AbstractHeader
	{
		public abstract int ByteCount { get; } // TODO: Use reflection?

		public byte[] Buffer { get; set; }
	}
}