namespace DevilDaggersAssetEditor.BinaryFileHandlers
{
	public class FileResult
	{
		public FileResult(string name, byte[] buffer)
		{
			Name = name;
			Buffer = buffer;
		}

		public string Name { get; set; }
		public byte[] Buffer { get; set; }

		public override string ToString()
			=> $"{Name} ({Buffer.Length} bytes)";
	}
}
