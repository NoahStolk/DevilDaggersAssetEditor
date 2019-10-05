using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.Previewers;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.AssetTabControlHandlers
{
	public class ParticlesAssetTabControlHandler : AbstractAssetTabControlHandler<ParticleAsset, ParticleAssetControl, ParticlePreviewer>
	{
		protected override string AssetTypeJsonFileName => "Particles";

		public ParticlesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGUI(ParticleAsset asset)
		{
			ParticleAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}