using System;

namespace DevilDaggersAssetCore
{
	public class ChunkInfo
	{
		public BinaryFileType BinaryFileType { get; set; }
		public Type Type { get; set; }
		public ushort[] BinaryTypes { get; set; }
		public string FileExtension { get; set; }
		public string FolderName { get; set; }

		public ChunkInfo(BinaryFileType binaryFileType, Type type, ushort[] binaryTypes, string fileExtension, string folderName)
		{
			BinaryFileType = binaryFileType;
			Type = type;
			BinaryTypes = binaryTypes;
			FileExtension = fileExtension;
			FolderName = folderName;
		}
	}
}