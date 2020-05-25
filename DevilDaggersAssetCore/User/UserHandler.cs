using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersAssetCore.User
{
	public sealed class UserHandler
	{
		// Must be fields since properties can't be used as out parameters.
		public UserSettings settings = new UserSettings();
		public UserCache cache = new UserCache();

		private static readonly Lazy<UserHandler> lazy = new Lazy<UserHandler>(() => new UserHandler());
		public static UserHandler Instance => lazy.Value;

		private UserHandler()
		{
		}

		public void SaveCache()
		{
			using StreamWriter sw = new StreamWriter(File.Create(UserCache.FileName));
			sw.Write(JsonConvert.SerializeObject(cache, Formatting.Indented));
		}

		public void LoadCache()
		{
			if (File.Exists(UserCache.FileName))
			{
				using StreamReader sr = new StreamReader(File.OpenRead(UserCache.FileName));
				cache = JsonConvert.DeserializeObject<UserCache>(sr.ReadToEnd());
			}
		}

		public void SaveSettings()
		{
			using StreamWriter sw = new StreamWriter(File.Create(UserSettings.FileName));
			sw.Write(JsonConvert.SerializeObject(settings, Formatting.Indented));
		}

		public void LoadSettings()
		{
			if (File.Exists(UserSettings.FileName))
			{
				using StreamReader sr = new StreamReader(File.OpenRead(UserSettings.FileName));
				settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
			}
		}
	}
}