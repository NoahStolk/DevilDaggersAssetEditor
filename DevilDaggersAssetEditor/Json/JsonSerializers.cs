using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Json
{
	public static class JsonSerializers
	{
		public static readonly JsonSerializerSettings DefaultSerializationSettings = new JsonSerializerSettings
		{
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		};
		public static readonly JsonSerializerSettings TypeNameSerializationSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Objects,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		};

		public static readonly JsonSerializer DefaultSerializer = JsonSerializer.Create(DefaultSerializationSettings);
		public static readonly JsonSerializer TypeNameSerializer = JsonSerializer.Create(TypeNameSerializationSettings);
	}
}
