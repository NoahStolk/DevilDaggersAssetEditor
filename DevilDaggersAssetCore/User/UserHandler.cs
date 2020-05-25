using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersAssetCore.User
{
	public sealed class UserHandler
	{
		// Must be a field since properties can't be used as out parameters.
		public UserSettings settings = new UserSettings();

		private static readonly Lazy<UserHandler> lazy = new Lazy<UserHandler>(() => new UserHandler());
		public static UserHandler Instance => lazy.Value;

		private UserHandler()
		{
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