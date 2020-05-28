using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ParticlesAssetTabControlHandler : AbstractAssetTabControlHandler<ParticleAsset, ParticleAssetRowControl, ParticleAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Particles";

		public ParticlesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(ParticleAsset asset)
		{
			ParticleAssetRowControl arc = AssetRowEntries.FirstOrDefault(a => a.AssetRowControlHandler.Asset == asset).AssetRowControlHandler.AssetRowControl;
			arc.Handler.UpdateGui();
		}
	}
}