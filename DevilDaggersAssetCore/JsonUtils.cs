﻿using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersAssetCore
{
	public static class JsonUtils
	{
		// MUST be initialized before JsonSerializers get initialized!
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

		public static void SerializeToFile(string path, object obj, bool includeType = false)
		{
			using StreamWriter sw = new StreamWriter(File.Create(path));
			using JsonTextWriter jtw = new JsonTextWriter(sw) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
			if (includeType)
				TypeNameSerializer.Serialize(jtw, obj);
			else
				DefaultSerializer.Serialize(jtw, obj);
		}

		public static T DeserializeFromFile<T>(string path, bool includeType)
		{
			using StreamReader sr = new StreamReader(File.OpenRead(path));
			if (includeType)
				return JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), TypeNameSerializationSettings);
			return JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), DefaultSerializationSettings);
		}
	}
}