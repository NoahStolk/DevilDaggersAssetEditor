using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class ParticleAssetControlHandler : AbstractAssetControlHandler<ParticleAsset, ParticleAssetControl>
	{
		public ParticleAssetControlHandler(ParticleAsset asset, ParticleAssetControl parent)
			: base(asset, parent, "Particle files (*.bin)|*.bin")
		{
		}

		public override void UpdateGUI()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}