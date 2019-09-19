using DevilDaggersCore.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace DevilDaggersAssetEditor.Code.Web
{
	public sealed class NetworkHandler
	{
		private const int Timeout = 7500; // 7.5 seconds

		public VersionResult VersionResult { get; set; } = new VersionResult(null, string.Empty, "Version has not yet been retrieved.");

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}

		public void RetrieveVersion()
		{
			string url = UrlUtils.GetToolVersions;
			try
			{
				string downloadString = string.Empty;
				using (TimeoutWebClient client = new TimeoutWebClient(Timeout))
					downloadString = client.DownloadString(url);
				List<Tool> tools = JsonConvert.DeserializeObject<List<Tool>>(downloadString);

				foreach (Tool tool in tools)
				{
					if (tool.Name == ApplicationUtils.ApplicationName)
					{
						VersionResult = new VersionResult(Version.Parse(tool.VersionNumber) <= ApplicationUtils.ApplicationVersionNumber, tool.VersionNumber, string.Empty);
						return;
					}
				}

				Error("Error retrieving latest version number", $"{ApplicationUtils.ApplicationName} not found in '{url}'.");
			}
			catch (WebException ex)
			{
				Error("Error retrieving latest version number", $"Could not connect to '{url}'.", ex);
			}
			catch (Exception ex)
			{
				Error("Unexpected error", $"An unexpected error occured while trying to retrieve the latest version number from '{url}'.", ex);
			}

			void Error(string title, string message, Exception ex = null)
			{
				VersionResult = new VersionResult(null, string.Empty, message);
				App.Instance.ShowError(title, message, ex);
			}
		}
	}
}