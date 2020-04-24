using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Code.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserSettings
	{
		public const string FileName = "user.json";

		[JsonProperty]
		public string DevilDaggersRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		[JsonProperty]
		public string ModsRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		[JsonProperty]
		public string AssetsRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
	}
}