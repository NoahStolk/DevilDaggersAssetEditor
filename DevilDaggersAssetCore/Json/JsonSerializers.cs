using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Json
{
	public static class JsonSerializers
	{
		public static readonly JsonSerializerSettings DefaultSerializationSettings = new JsonSerializerSettings
		{
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};
		public static readonly JsonSerializerSettings TypeNameSerializationSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Objects,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};

		public static readonly JsonSerializer DefaultSerializer;
		public static readonly JsonSerializer TypeNameSerializer;

		static JsonSerializers()
		{
			DefaultSerializer = JsonSerializer.Create(DefaultSerializationSettings);
			TypeNameSerializer = JsonSerializer.Create(TypeNameSerializationSettings);
		}
	}
}