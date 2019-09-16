using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ParticleExtract
{
	public static class Program
	{
		private static readonly Random random = new Random();

		public static void Main()
		{
			byte[] fileBuffer = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd\particle0");

			Dictionary<string, byte[]> particleChunks = BytesToDict(fileBuffer);

			File.WriteAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd\particle", DictToBytes(ShuffleParticles(particleChunks)));

			//SaveHexRepresentationFile(particleChunks);
		}

		private static Dictionary<string, byte[]> ShuffleParticles(Dictionary<string, byte[]> particleChunks)
		{
			Dictionary<string, byte[]> shuffled = new Dictionary<string, byte[]>();
			List<int> indices = Enumerable.Range(0, particleChunks.Count).OrderBy(p => random.Next()).ToList();
			int i = 0;
			foreach (KeyValuePair<string, byte[]> kvp in particleChunks)
				shuffled[kvp.Key] = particleChunks.Values.ElementAt(indices[i++]);
			return shuffled;
		}

		private static Dictionary<string, byte[]> BytesToDict(byte[] fileBuffer)
		{
			Dictionary<string, byte[]> particleChunks = new Dictionary<string, byte[]>();

			int i = 8;
			while (i < fileBuffer.Length)
			{
				string name = ReadNullTerminatedString(fileBuffer, i);
				i += name.Length;

				byte[] chunkBuffer = new byte[188];
				Buffer.BlockCopy(fileBuffer, i, chunkBuffer, 0, chunkBuffer.Length);

				i += 188;

				particleChunks[name] = chunkBuffer;
			}

			return particleChunks;
		}

		private static byte[] DictToBytes(Dictionary<string, byte[]> dict)
		{
			byte[] fileBuffer;
			using (MemoryStream stream = new MemoryStream())
			{
				stream.Write(BitConverter.GetBytes(4), 0, sizeof(int));
				stream.Write(BitConverter.GetBytes(dict.Count), 0, sizeof(int));
				foreach (KeyValuePair<string, byte[]> kvp in dict)
				{
					stream.Write(Encoding.Default.GetBytes(kvp.Key), 0, kvp.Key.Length);
					stream.Write(kvp.Value, 0, kvp.Value.Length);
				}
				fileBuffer = stream.ToArray();
			}

			return fileBuffer;
		}

		private static void SaveHexRepresentationFile(Dictionary<string, byte[]> particleChunks)
		{
			Dictionary<string, string> hex = new Dictionary<string, string>();
			foreach (KeyValuePair<string, byte[]> kvp in particleChunks)
				hex[kvp.Key] = BitConverter.ToString(kvp.Value).Replace(" - ", "");

			SerializeToFile(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\Extracted\particle\Particle.json", hex);
		}

		/// <summary>
		/// Reads a null terminated string from a buffer and returns it as a string object (excluding the null terminator itself).
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">The starting offset to start reading from within the buffer.</param>
		/// <returns>The null terminated string.</returns>
		private static string ReadNullTerminatedString(byte[] buffer, int offset)
		{
			StringBuilder name = new StringBuilder();
			for (int i = offset; i < buffer.Length; i++)
			{
				char c = (char)buffer[i];
				if (c == '\0')
					return name.ToString();
				name.Append(c);
			}
			throw new Exception($"Null terminator not observed in buffer with length {buffer.Length} starting from offset {offset}.");
		}

		private static void SerializeToFile(string path, object obj)
		{
			using (StreamWriter sw = new StreamWriter(File.Create(path)))
			using (JsonTextWriter jtw = new JsonTextWriter(sw) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 })
				DefaultSerializer.Serialize(jtw, obj);
		}

		private static readonly JsonSerializerSettings DefaultSerializationSettings = new JsonSerializerSettings()
		{
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};
		private static readonly JsonSerializerSettings TypeNameSerializationSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Objects,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};

		public static readonly JsonSerializer DefaultSerializer = JsonSerializer.Create(DefaultSerializationSettings);
		public static readonly JsonSerializer TypeNameSerializer = JsonSerializer.Create(TypeNameSerializationSettings);
	}
}