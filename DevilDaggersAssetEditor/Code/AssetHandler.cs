using DevilDaggersAssetCore.Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersAssetEditor.Code
{
	public sealed class AssetHandler
	{
		public List<AudioAsset> AudioAssets { get; private set; } = new List<AudioAsset>();

		private static readonly Lazy<AssetHandler> lazy = new Lazy<AssetHandler>(() => new AssetHandler());
		public static AssetHandler Instance => lazy.Value;

		private AssetHandler()
		{
			string stream;
			using (StreamReader sr = new StreamReader(Utils.Assembly.GetManifestResourceStream("DevilDaggersAssetEditor.Content.AssetInfo.Audio.json")))
				stream = sr.ReadToEnd();
			AudioAssets = JsonConvert.DeserializeObject<List<AudioAsset>>(stream);
		}
	}
}