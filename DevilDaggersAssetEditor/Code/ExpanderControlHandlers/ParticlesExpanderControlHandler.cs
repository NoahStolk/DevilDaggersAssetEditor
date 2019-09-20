using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.ExpanderControlHandlers
{
	public class ParticlesExpanderControlHandler : AbstractExpanderControlHandler<ParticleAsset, ParticleAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Particles";

		public ParticlesExpanderControlHandler(BinaryFileType binaryFileType)
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