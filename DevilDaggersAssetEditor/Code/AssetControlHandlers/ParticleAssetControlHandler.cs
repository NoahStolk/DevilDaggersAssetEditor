using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ParticleAssetControlHandler : AbstractAssetControlHandler<ParticleAsset, ParticleAssetControl>
	{
		public ParticleAssetControlHandler(ParticleAsset asset, ParticleAssetControl parent)
			: base(asset, parent, "Particle files (*.bin)|*.bin")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}