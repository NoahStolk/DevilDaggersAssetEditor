using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class ParticlesAssetTabControlHandler : AbstractAssetTabControlHandler<ParticleAssetRowControlHandler>
	{
		public ParticlesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType, AssetType.Particle)
		{
		}

		protected override string AssetTypeJsonFileName => "Particles";
	}
}