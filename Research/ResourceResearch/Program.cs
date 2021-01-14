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
			PrintFirstModelVertex(3);
		}

		private static void PrintFirstModelVertex(int count)
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd0");

			ResourceFileHandler handler = new ResourceFileHandler(BinaryFileType.Dd);
			byte[] tocBuffer = handler.ReadTocBuffer(fileBuffer);

			int i = 0;
			int j = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				byte type = tocBuffer[i];
				string name = Utils.ReadNullTerminatedString(tocBuffer, i + 2);

				if (type != 0x01) // Not a model
				{
					i += 14 + name.Length + 1;
					continue;
				}

				byte[] buf = new byte[14 + name.Length + 1]; // + 1 to include null terminator
				Buffer.BlockCopy(tocBuffer, i, buf, 0, buf.Length);

				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + name.Length + 3);
				i += buf.Length;

				Console.WriteLine(name);
				Console.WriteLine($"{GetHexRepresentation(fileBuffer, (int)startOffset + 10, 32)}");
				if (++j >= count)
					break;
			}
		}

		private static void PrintModelHeaders(int count)
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd0");

			ResourceFileHandler handler = new ResourceFileHandler(BinaryFileType.Dd);
			byte[] tocBuffer = handler.ReadTocBuffer(fileBuffer);

			int i = 0;
			int j = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				byte type = tocBuffer[i];
				string name = Utils.ReadNullTerminatedString(tocBuffer, i + 2);

				if (type != 0x01) // Not a model
				{
					i += 14 + name.Length + 1;
					continue;
				}

				byte[] buf = new byte[14 + name.Length + 1]; // + 1 to include null terminator
				Buffer.BlockCopy(tocBuffer, i, buf, 0, buf.Length);

				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + name.Length + 3);
				i += buf.Length;

				uint vertexCount = BitConverter.ToUInt32(fileBuffer, (int)startOffset);
				uint indexCount = BitConverter.ToUInt32(fileBuffer, (int)startOffset + sizeof(uint));
				ushort unknown = BitConverter.ToUInt16(fileBuffer, (int)startOffset + sizeof(uint) + sizeof(uint));

				Console.WriteLine(name);
				Console.WriteLine($"{GetHexRepresentation(fileBuffer, (int)startOffset, 10)}");
				Console.WriteLine($"{vertexCount} {indexCount} {unknown}\n");
				if (++j >= count)
					break;
			}
		}

		private static void PrintShaderHeaders(int count)
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd0");

			ResourceFileHandler handler = new ResourceFileHandler(BinaryFileType.Dd);
			byte[] tocBuffer = handler.ReadTocBuffer(fileBuffer);

			int i = 0;
			int j = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				byte type = tocBuffer[i];
				string name = Utils.ReadNullTerminatedString(tocBuffer, i + 2);

				if (type != 0x10) // Not a shader
				{
					i += 14 + name.Length + 1;
					continue;
				}

				byte[] buf = new byte[14 + name.Length + 1]; // + 1 to include null terminator
				Buffer.BlockCopy(tocBuffer, i, buf, 0, buf.Length);

				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + name.Length + 3);
				i += buf.Length;

				uint shaderNameLength = BitConverter.ToUInt32(fileBuffer, (int)startOffset);
				uint vertexLength = BitConverter.ToUInt32(fileBuffer, (int)startOffset + sizeof(uint));
				uint fragmentLength = BitConverter.ToUInt32(fileBuffer, (int)startOffset + sizeof(uint) + sizeof(uint));

				Console.WriteLine(name);
				Console.WriteLine($"{GetHexRepresentation(fileBuffer, (int)startOffset, 12)}");
				Console.WriteLine($"{shaderNameLength} {vertexLength} {fragmentLength}\n");
				if (++j >= count)
					break;
			}
		}

		private static void PrintTextureHeaders(int count)
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd0");

			ResourceFileHandler handler = new ResourceFileHandler(BinaryFileType.Dd);
			byte[] tocBuffer = handler.ReadTocBuffer(fileBuffer);

			int i = 0;
			int j = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				byte type = tocBuffer[i];
				string name = Utils.ReadNullTerminatedString(tocBuffer, i + 2);

				if (type != 0x02) // Not a texture
				{
					i += 14 + name.Length + 1;
					continue;
				}

				byte[] buf = new byte[14 + name.Length + 1]; // + 1 to include null terminator
				Buffer.BlockCopy(tocBuffer, i, buf, 0, buf.Length);

				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + name.Length + 3);
				i += buf.Length;

				ushort unknown = BitConverter.ToUInt16(fileBuffer, (int)startOffset);
				uint width = BitConverter.ToUInt32(fileBuffer, (int)startOffset + sizeof(ushort));
				uint height = BitConverter.ToUInt32(fileBuffer, (int)startOffset + sizeof(ushort) + sizeof(uint));
				byte mipmaps = fileBuffer[(int)startOffset + sizeof(ushort) + sizeof(uint) + sizeof(uint)];

				Console.WriteLine(name);
				Console.WriteLine($"{GetHexRepresentation(fileBuffer, (int)startOffset, 11)}");
				Console.WriteLine($"{unknown} {width} {height} {mipmaps}\n");
				if (++j >= count)
					break;
			}
		}

		/// <summary>
		/// Used to figure out if fragment shaders are actually used. (They're not; all sizes are 0.)
		/// </summary>
		private static void PrintFragmentShaderTocEntries()
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\res\dd0");

			ResourceFileHandler handler = new ResourceFileHandler(BinaryFileType.Dd);
			byte[] tocBuffer = handler.ReadTocBuffer(fileBuffer);

			int i = 0;
			while (i < tocBuffer.Length - 14) // TODO: Might still get out of range maybe... (14 bytes per chunk, but name length is variable)
			{
				byte type = tocBuffer[i];
				string name = Utils.ReadNullTerminatedString(tocBuffer, i + 2);

				if (type != 0x11) // Not a fragment shader
				{
					i += 14 + name.Length + 1;
					continue;
				}

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

		private static void PrintOneOfEachTocEntry()
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

				if (types.Contains(type))
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
