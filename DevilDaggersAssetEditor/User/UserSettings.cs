using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.User;

[JsonObject(MemberSerialization.OptIn)]
public class UserSettings
{
	public const string PathDefault = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

	private const string _fileName = "settings.json";

	public static string FileDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevilDaggersAssetEditor");
	public static string FilePath => Path.Combine(FileDirectory, _fileName);

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
	public bool OpenModFolderAfterExtracting { get; set; }

	[JsonProperty]
	public uint TextureSizeLimit { get; set; } = 512;
}
