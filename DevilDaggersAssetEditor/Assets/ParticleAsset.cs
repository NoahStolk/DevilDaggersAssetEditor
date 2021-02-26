using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ParticleAsset : AbstractAsset
	{
		public ParticleAsset(string assetName, string description, bool isProhibited, List<string> tags)
			: base(assetName, AssetType.Particle, description, isProhibited, tags)
		{
		}

		public override UserAsset ToUserAsset()
			=> new(AssetType.Particle, AssetName, EditorPath);
	}
}
