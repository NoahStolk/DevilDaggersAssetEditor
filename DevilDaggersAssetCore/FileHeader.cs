namespace DevilDaggersAssetCore
{
	public class FileHeader
	{
		public uint MagicNumber1 { get; set; }
		public uint MagicNumber2 { get; set; }
		public uint TocSize { get; set; }

		public FileHeader(uint magicNumber1, uint magicNumber2, uint tocSize)
		{
			MagicNumber1 = magicNumber1;
			MagicNumber2 = magicNumber2;
			TocSize = tocSize;
		}
	}
}