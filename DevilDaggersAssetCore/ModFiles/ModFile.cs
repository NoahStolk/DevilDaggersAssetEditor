using System;
using System.Collections.Generic;

namespace DevilDaggersAssetCore.ModFiles
{
	public class ModFile
	{
		public Version DDAEVersion { get; set; }
		public bool HasRelativePaths { get; set; }
		public List<GenericUserAsset> Assets { get; set; }

		public ModFile(Version ddaeVersion, bool hasRelativePaths, List<GenericUserAsset> assets)
		{
			DDAEVersion = ddaeVersion;
			HasRelativePaths = hasRelativePaths;
			Assets = assets;
		}
	}
}