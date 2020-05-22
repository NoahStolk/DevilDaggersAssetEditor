namespace DevilDaggersAssetCore.Headers
{
	public class ParticleHeader : AbstractHeader
	{
		public override int ByteCount => Buffer.Length;

		public ParticleHeader(byte[] headerBuffer)
		{
			Buffer = headerBuffer;
		}
	}
}