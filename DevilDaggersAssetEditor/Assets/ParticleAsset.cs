using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ParticleAsset : AbstractAsset
	{
		public ParticleAsset(string assetName, string description, List<string> tags, bool isProhibited)
			: base(assetName, AssetType.Particle, description, tags, isProhibited)
		{
		}

		public override UserAsset ToUserAsset()
			=> new(AssetType.Particle, AssetName, EditorPath);
	}
}
