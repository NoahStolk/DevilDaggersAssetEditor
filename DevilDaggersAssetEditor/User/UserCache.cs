using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.User
{
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
		public string MakeBinaryCoreName { get; set; } = string.Empty;
		[JsonProperty]
		public string MakeBinaryDdName { get; set; } = string.Empty;
	}
}
