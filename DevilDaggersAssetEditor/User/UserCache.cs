using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetEditor.User;

public class UserCache
{
	private const string _fileName = "cache.json";

	public static string FileDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevilDaggersAssetEditor");
	public static string FilePath => Path.Combine(FileDirectory, _fileName);

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

	[JsonProperty]
	public string MakeBinaryName { get; set; } = string.Empty;
	[JsonProperty]
	public string MakeBinaryAudioName { get; set; } = string.Empty;
	[JsonProperty]
	public string MakeBinaryDdName { get; set; } = string.Empty;

	[JsonProperty]
	public int ModManagerActiveTabIndex { get; set; }

	[JsonProperty]
	public string? DownloadAuthorFilter { get; set; }
	[JsonProperty]
	public string? DownloadModFilter { get; set; }
	[JsonProperty]
	public int? DownloadSortingIndex { get; set; }
	[JsonProperty]
	public List<bool>? DownloadSortingDirections { get; set; }

	[JsonProperty]
	public bool ImportAudioAudio { get; set; } = true;
	[JsonProperty]
	public bool ImportCoreShaders { get; set; } = true;
	[JsonProperty]
	public bool ImportDdShaders { get; set; } = true;
	[JsonProperty]
	public bool ImportDdTextures { get; set; } = true;
	[JsonProperty]
	public bool ImportDdModels { get; set; } = true;
	[JsonProperty]
	public bool ImportDdModelBindings { get; set; } = true;

	[JsonProperty]
	public bool ImportAudioAudioAllDirectories { get; set; } = true;
	[JsonProperty]
	public bool ImportCoreShadersAllDirectories { get; set; } = true;
	[JsonProperty]
	public bool ImportDdShadersAllDirectories { get; set; } = true;
	[JsonProperty]
	public bool ImportDdTexturesAllDirectories { get; set; } = true;
	[JsonProperty]
	public bool ImportDdModelsAllDirectories { get; set; } = true;
	[JsonProperty]
	public bool ImportDdModelBindingsAllDirectories { get; set; } = true;
}
