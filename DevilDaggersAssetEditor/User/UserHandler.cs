using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.User
{
	public sealed class UserHandler
	{
		private static readonly Lazy<UserHandler> _lazy = new(() => new());

		private UserHandler()
		{
		}

		public static UserHandler Instance => _lazy.Value;

		public UserCache Cache { get; set; } = new();
		public UserSettings Settings { get; set; } = new();

		public void ReadCache()
		{
			if (!File.Exists(UserCache.FileName))
				return;

			using StreamReader sr = new(File.OpenRead(UserCache.FileName));
			Cache = JsonConvert.DeserializeObject<UserCache>(sr.ReadToEnd());
		}

		public void ReadSettings()
		{
			if (!File.Exists(UserSettings.FileName))
				return;

			using StreamReader sr = new(File.OpenRead(UserSettings.FileName));
			Settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
		}

		public void SaveCache()
		{
			using StreamWriter sw = new(File.Create(UserCache.FileName));
			sw.Write(JsonConvert.SerializeObject(Cache, Formatting.Indented));
		}

		public void SaveSettings()
		{
			using StreamWriter sw = new(File.Create(UserSettings.FileName));
			sw.Write(JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}
	}
}
