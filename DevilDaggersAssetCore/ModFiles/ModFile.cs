using System;
using System.Collections.Generic;

namespace DevilDaggersAssetCore.ModFiles
{
	public class ModFile
	{
		public Version DdaeVersion { get; set; }
		public bool HasRelativePaths { get; set; }
		public List<AbstractUserAsset> Assets { get; set; }

		public ModFile(Version ddaeVersion, bool hasRelativePaths, List<AbstractUserAsset> assets)
		{
			DdaeVersion = ddaeVersion;
			HasRelativePaths = hasRelativePaths;
			Assets = assets;
		}
	}
}