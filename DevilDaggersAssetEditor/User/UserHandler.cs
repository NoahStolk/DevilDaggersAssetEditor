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

		public UserSettings Settings { get; set; } = new();
		public UserCache Cache { get; set; } = new();

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
