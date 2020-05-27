using Newtonsoft.Json;

namespace DevilDaggersAssetCore.User
{
	public class UserCache
	{
		public const string FileName = "cache.json";

		[JsonProperty]
		public string OpenedModFilePath { get; set; }
		[JsonProperty]
		public int ActiveTabIndex { get; set; }
		[JsonProperty]
		public int WindowWidth { get; set; }
		[JsonProperty]
		public int WindowHeight { get; set; }
		[JsonProperty]
		public bool WindowIsFullScreen { get; set; }
	}
}