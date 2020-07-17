using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersAssetCore.Json
{
	public static class JsonFileUtils
	{
		public static bool TrySerializeToFile(string path, object obj, bool includeType)
		{
			try
			{
				SerializeToFile(path, obj, includeType);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static void SerializeToFile(string path, object obj, bool includeType)
		{
			using StreamWriter sw = new StreamWriter(File.Create(path));
			using JsonTextWriter jtw = new JsonTextWriter(sw) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
			if (includeType)
				JsonSerializers.TypeNameSerializer.Serialize(jtw, obj);
			else
				JsonSerializers.DefaultSerializer.Serialize(jtw, obj);
		}

		public static bool TryDeserializeFromFile<T>(string path, bool includeType, out T result)
		{
			try
			{
				result = DeserializeFromFile<T>(path, includeType);
				return true;
			}
			catch
			{
				result = default;
				return false;
			}
		}

		public static T DeserializeFromFile<T>(string path, bool includeType)
		{
			using StreamReader sr = new StreamReader(File.OpenRead(path));
			if (includeType)
				return JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), JsonSerializers.TypeNameSerializationSettings);
			return JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), JsonSerializers.DefaultSerializationSettings);
		}
	}
}