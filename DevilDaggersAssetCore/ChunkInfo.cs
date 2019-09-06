using System;

namespace DevilDaggersAssetCore
{
	public class ChunkInfo
	{
		public Type Type { get; set; }
		public ushort[] BinaryTypes { get; set; }
		public string FileExtension { get; set; }
		public string FolderName { get; set; }

		public ChunkInfo(Type type, ushort[] binaryTypes, string fileExtension, string folderName)
		{
			Type = type;
			BinaryTypes = binaryTypes;
			FileExtension = fileExtension;
			FolderName = folderName;
		}
	}
}