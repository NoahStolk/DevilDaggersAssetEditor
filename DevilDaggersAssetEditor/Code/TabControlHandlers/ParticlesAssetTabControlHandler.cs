using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ParticlesAssetTabControlHandler : AbstractAssetTabControlHandler<ParticleAsset, ParticleAssetRowControl, ParticleAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Particles";

		public ParticlesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}
	}
}