using Newtonsoft.Json;

namespace DevilDaggersAssetCore.User
{
	public class UserCache
	{
		public const string FileName = "cache.json";

		[JsonProperty]
		public string LastOpenedModFile { get; set; }
		[JsonProperty]
		public int LastActiveTabIndex { get; set; }
	}
}