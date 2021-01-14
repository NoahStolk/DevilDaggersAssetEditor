using DevilDaggersAssetEditor.Wpf.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DevilDaggersAssetEditor.Wpf.Network
{
	public sealed class NetworkHandler
	{
#if TESTING
		public static readonly string BaseUrl = "http://localhost:2963";
#else
		public static readonly string BaseUrl = "https://devildaggers.info";
#endif

		private static readonly Lazy<NetworkHandler> _lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());

		private NetworkHandler()
		{
			HttpClient httpClient = new HttpClient
			{
				BaseAddress = new Uri(BaseUrl),
			};
			ApiClient = new DevilDaggersInfoApiClient(httpClient);
		}

		public static NetworkHandler Instance => _lazy.Value;

		public DevilDaggersInfoApiClient ApiClient { get; }

		public Tool? Tool { get; private set; }

		public bool GetOnlineTool()
		{
			try
			{
				List<Tool> tools = ApiClient.Tools_GetToolsAsync(App.ApplicationName).Result;
				Tool = tools.First();

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving tool information", "An error occurred while attempting to retrieve tool information from the API.", ex);
				return false;
			}
		}
	}
}
