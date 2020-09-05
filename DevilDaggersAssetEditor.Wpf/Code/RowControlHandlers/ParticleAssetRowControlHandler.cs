using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers
{
	public class ParticleAssetRowControlHandler : AbstractAssetRowControlHandler<ParticleAsset, ParticleAssetRowControl>
	{
		public ParticleAssetRowControlHandler(ParticleAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override string OpenDialogFilter => "Particle files (*.bin)|*.bin";

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
		}
	}
}