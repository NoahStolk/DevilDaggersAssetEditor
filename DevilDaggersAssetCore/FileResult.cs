namespace DevilDaggersAssetCore
{
	public class FileResult
	{
		public string Name { get; set; }
		public byte[] Buffer { get; set; }

		public FileResult(string name, byte[] buffer)
		{
			Name = name;
			Buffer = buffer;
		}
	}
}