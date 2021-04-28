using DevilDaggersAssetEditor.Wpf.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevilDaggersAssetEditor.Wpf.Network
{
	public sealed class NetworkHandler
	{
#if TESTING
		public static readonly string BaseUrl = "http://localhost:2963";
#else
		public static readonly string BaseUrl = "https://devildaggers.info";
#endif

		private static readonly Lazy<NetworkHandler> _lazy = new(() => new());

		private NetworkHandler()
		{
			ApiClient = new(new() { BaseAddress = new(BaseUrl) });
		}

		public static NetworkHandler Instance => _lazy.Value;

		public DevilDaggersInfoApiClient ApiClient { get; }

		public Tool? Tool { get; private set; }

		public List<Mod> Mods { get; } = new();

		public bool GetOnlineTool()
		{
			try
			{
				List<Tool> tools = ApiClient.Tools_GetToolsAsync(App.ApplicationName).Result;
				Tool = tools[0];

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving tool information", "An error occurred while attempting to retrieve tool information from the API.", ex);
				return false;
			}
		}

		public async Task<bool> RetrieveModList()
		{
			try
			{
				Mods.Clear();
				Mods.AddRange(await ApiClient.Mods_GetModsAsync(null, null, true));

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving mod list", "An error occurred while attempting to retrieve mods from the API.", ex);
				return false;
			}
		}
	}
}
