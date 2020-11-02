using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;

namespace DevilDaggersAssetEditor.Assets
{
	public class ParticleAsset : AbstractAsset
	{
		public ParticleAsset(string assetName, string description, List<string> tags)
			: base(assetName, AssetType.Particle, description, tags)
		{
		}

		public override UserAsset ToUserAsset()
			=> new UserAsset(AssetType.Particle, AssetName, EditorPath);
	}
}