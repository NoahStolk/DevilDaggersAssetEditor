using DevilDaggersAssetCore;
using DevilDaggersAssetCore.BinaryFileHandlers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ResourceResearch
{
	public static class Program
	{
		public static void Main()
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd0");

			ResourceFileHandler handler = new ResourceFileHandler(BinaryFileType.Dd);
			byte[] tocBuffer = handler.ReadTocBuffer(fileBuffer);

			List<byte> types = new List<byte>();

			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				byte type = tocBuffer[i];
				string name = Utils.ReadNullTerminatedString(tocBuffer, i + 2);

				if (type != 17)
				{
					i += 14 + name.Length + 1;
					continue;
				}

				types.Add(type);

				byte[] buf = new byte[14 + name.Length + 1]; // + 1 to include null terminator
				Buffer.BlockCopy(tocBuffer, i, buf, 0, buf.Length);

				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + name.Length + 3);
				uint size = BitConverter.ToUInt32(tocBuffer, i + name.Length + 7);
				uint unknown = BitConverter.ToUInt32(tocBuffer, i + name.Length + 11);
				i += buf.Length;

				Console.WriteLine($"{GetHexRepresentation(buf, 0, sizeof(byte))} {GetHexRepresentation(buf, 2, name.Length + 1)} {GetHexRepresentation(buf, name.Length + 3, sizeof(uint))} {GetHexRepresentation(buf, name.Length + 7, sizeof(uint))} {GetHexRepresentation(buf, name.Length + 11, sizeof(uint))}");
				Console.WriteLine($"{type} {name} {startOffset} {size} {unknown}\n");
			}
		}

		public static string GetHexRepresentation(byte[] buffer, int startIndex, int length) => BitConverter.ToString(buffer, startIndex, length).Replace("-", "");
	}
}