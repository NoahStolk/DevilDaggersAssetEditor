using System;

namespace DevilDaggersAssetCore
{
	public class ChunkInfo
	{
		public BinaryFileName BinaryFileName { get; set; }
		public Type Type { get; set; }
		public ushort[] BinaryTypes { get; set; }
		public string FileExtension { get; set; }
		public string FolderName { get; set; }

		public ChunkInfo(BinaryFileName binaryFileName, Type type, ushort[] binaryTypes, string fileExtension, string folderName)
		{
			BinaryFileName = binaryFileName;
			Type = type;
			BinaryTypes = binaryTypes;
			FileExtension = fileExtension;
			FolderName = folderName;
		}
	}
}