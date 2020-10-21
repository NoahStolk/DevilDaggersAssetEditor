using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Wpf.RowControlHandlers
{
	public class ParticleAssetRowControlHandler : AssetRowControlHandler
	{
		public ParticleAssetRowControlHandler(AbstractAsset asset, bool isEven)
			: base(asset, AssetType.Particle, isEven, "Particle files (*.bin)|*.bin")
		{
		}
	}
}