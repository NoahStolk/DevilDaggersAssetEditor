using System.Collections.Immutable;

namespace DevilDaggersAssetEditor.New
{
	public class FileResult
	{
		public FileResult(string fileName, ImmutableArray<byte> contents)
		{
			FileName = fileName;
			Contents = contents;
		}

		public string FileName { get; }
		public ImmutableArray<byte> Contents { get; }
	}
}