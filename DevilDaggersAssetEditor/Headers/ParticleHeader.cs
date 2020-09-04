namespace DevilDaggersAssetEditor.Headers
{
	public class ParticleHeader : AbstractHeader
	{
		public override uint ByteCount => (uint)Buffer.Length;

		public ParticleHeader(byte[] headerBuffer)
		{
			Buffer = headerBuffer;
		}
	}
}