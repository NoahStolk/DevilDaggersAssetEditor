using Newtonsoft.Json;

namespace DevilDaggersAssetCore.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserSettings
	{
		public const string FileName = "user.json";
		public const string PathDefault = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		[JsonProperty]
		public string DevilDaggersRootFolder { get; set; } = PathDefault;
		[JsonProperty]
		public bool EnableDevilDaggersRootFolder { get; set; }

		[JsonProperty]
		public string ModsRootFolder { get; set; } = PathDefault;
		[JsonProperty]
		public bool EnableModsRootFolder { get; set; }

		[JsonProperty]
		public string AssetsRootFolder { get; set; } = PathDefault;
		[JsonProperty]
		public bool EnableAssetsRootFolder { get; set; }

		[JsonProperty]
		public bool CreateModFileWhenExtracting { get; set; }

		[JsonProperty]
		public bool OpenModFolderAfterExtracting { get; set; }

		[JsonProperty]
		public uint TextureSizeLimit { get; set; } = 512;
	}
}