using Newtonsoft.Json;

namespace DevilDaggersAssetEditor.Code.User
{
	[JsonObject(MemberSerialization.OptIn)]
	internal class UserSettings
	{
		internal const string FileName = "user.json";

		[JsonProperty]
		internal string DevilDaggersRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		[JsonProperty]
		internal string ModsRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		[JsonProperty]
		internal string AssetsRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
	}
}