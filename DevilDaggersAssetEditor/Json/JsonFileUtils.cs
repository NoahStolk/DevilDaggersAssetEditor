using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersAssetEditor.Json
{
	public static class JsonFileUtils
	{
		public static void SerializeToFile(string path, object obj, bool includeType)
		{
			using StreamWriter sw = new StreamWriter(File.Create(path));
			using JsonTextWriter jtw = new JsonTextWriter(sw) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
			if (includeType)
				JsonSerializers.TypeNameSerializer.Serialize(jtw, obj);
			else
				JsonSerializers.DefaultSerializer.Serialize(jtw, obj);
		}

		public static T? TryDeserializeFromFile<T>(string path, bool includeType)
			where T : class
		{
			try
			{
				using StreamReader sr = new StreamReader(File.OpenRead(path));
				if (includeType)
					return JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), JsonSerializers.TypeNameSerializationSettings);
				return JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), JsonSerializers.DefaultSerializationSettings);
			}
			catch
			{
				return null;
			}
		}
	}
}
