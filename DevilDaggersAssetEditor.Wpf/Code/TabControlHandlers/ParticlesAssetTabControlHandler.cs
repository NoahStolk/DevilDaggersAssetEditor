using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Wpf.Code.TabControlHandlers
{
	public class ParticlesAssetTabControlHandler : AbstractAssetTabControlHandler<ParticleAsset, ParticleAssetRowControl, ParticleAssetRowControlHandler>
	{
		public ParticlesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		protected override string AssetTypeJsonFileName => "Particles";
	}
}