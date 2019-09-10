using DevilDaggersAssetCore.Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
			using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevilDaggersAssetCore.Content.Audio.json")))
				stream = sr.ReadToEnd();
			AudioAssets = JsonConvert.DeserializeObject<List<AudioAsset>>(stream);
		}
	}
}