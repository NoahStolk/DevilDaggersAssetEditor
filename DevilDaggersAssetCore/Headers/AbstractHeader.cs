namespace DevilDaggersAssetCore.Headers
{
	public abstract class AbstractHeader
	{
		public abstract int ByteCount { get; }

		public byte[] Buffer { get; protected set; }
	}
}