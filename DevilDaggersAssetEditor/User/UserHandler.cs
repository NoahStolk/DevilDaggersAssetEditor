using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.User
{
	public sealed class UserHandler
	{
		private static readonly Lazy<UserHandler> _lazy = new Lazy<UserHandler>(() => new UserHandler());

		private UserHandler()
		{
		}

		public static UserHandler Instance => _lazy.Value;

		public UserSettings Settings { get; set; } = new UserSettings();
		public UserCache Cache { get; set; } = new UserCache();

		public void SaveCache()
		{
			using StreamWriter sw = new StreamWriter(File.Create(UserCache.FileName));
			sw.Write(JsonConvert.SerializeObject(Cache, Formatting.Indented));
		}

		public void SaveSettings()
		{
			using StreamWriter sw = new StreamWriter(File.Create(UserSettings.FileName));
			sw.Write(JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}
	}
}