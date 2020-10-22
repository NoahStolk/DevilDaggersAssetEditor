using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.User
{
	public class UserCache
	{
		public const string FileName = "cache.json";

		[JsonProperty]
		public string OpenedModFilePath { get; set; } = string.Empty;
		[JsonProperty]
		public int ActiveTabIndex { get; set; }
		[JsonProperty]
		public int WindowWidth { get; set; }
		[JsonProperty]
		public int WindowHeight { get; set; }
		[JsonProperty]
		public bool WindowIsFullScreen { get; set; }
		[JsonProperty]
		public bool AudioPlayerIsAutoplayEnabled { get; set; }
	}
}