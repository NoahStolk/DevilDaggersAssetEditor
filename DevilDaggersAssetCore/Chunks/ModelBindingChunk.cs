namespace DevilDaggersAssetCore.Chunks
{
	public class ModelBindingChunk : AbstractChunk
	{
		public override string FileExtension => ".txt";

		public ModelBindingChunk(string name, uint startOffset, uint size, uint unknown)
			: base(name, startOffset, size, unknown)
		{
		}
	}
}