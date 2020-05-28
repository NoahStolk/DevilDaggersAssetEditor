using Newtonsoft.Json;

namespace DevilDaggersAssetCore.User
{
	public class UserCache
	{
		public const string FileName = "cache.json";

		[JsonProperty]
		public string OpenedAudioModFilePath { get; set; }
		[JsonProperty]
		public string OpenedCoreModFilePath { get; set; }
		[JsonProperty]
		public string OpenedDdModFilePath { get; set; }
		[JsonProperty]
		public string OpenedParticleModFilePath { get; set; }
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