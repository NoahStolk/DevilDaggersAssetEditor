namespace DevilDaggersAssetEditor.Headers
{
	public abstract class AbstractHeader
	{
		public abstract uint ByteCount { get; }

		public byte[] Buffer { get; protected set; }
	}
}