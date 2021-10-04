using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Wpf.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
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
				Tool = ApiClient.Tools_GetToolAsync(App.ApplicationName).Result;

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
				Mods.AddRange(await ApiClient.Mods_GetModsForDdaeAsync(null, null, true));

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving mod list", "An error occurred while attempting to retrieve mods from the API.", ex);
				return false;
			}
		}

		public async Task<bool> RetrieveAssetInfo()
		{
			try
			{
				foreach (KeyValuePair<string, List<AssetInfo>> kvp in await ApiClient.Assets_GetAssetInfoForDdaeAsync())
				{
					List<AbstractAsset>? assets = (kvp.Key switch
					{
						"audioAudio" => AssetContainer.Instance.AudioAudioAssets.Cast<AbstractAsset>(),
						"coreShaders" => AssetContainer.Instance.CoreShadersAssets.Cast<AbstractAsset>(),
						"ddModelBindings" => AssetContainer.Instance.DdModelBindingsAssets.Cast<AbstractAsset>(),
						"ddModels" => AssetContainer.Instance.DdModelsAssets.Cast<AbstractAsset>(),
						"ddShaders" => AssetContainer.Instance.DdShadersAssets.Cast<AbstractAsset>(),
						"ddTextures" => AssetContainer.Instance.DdTexturesAssets.Cast<AbstractAsset>(),
						_ => null,
					})?.ToList();

					if (assets == null)
						continue;

					foreach (AbstractAsset asset in assets)
					{
						AssetInfo? assetInfo = kvp.Value.Find(ai => ai.Name == asset.AssetName);
						asset.Description = assetInfo?.Description;
						asset.Tags = assetInfo?.Tags ?? new();
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Error retrieving asset info", "An error occurred while attempting to retrieve asset info from the API.", ex);
				return false;
			}
		}
	}
}
