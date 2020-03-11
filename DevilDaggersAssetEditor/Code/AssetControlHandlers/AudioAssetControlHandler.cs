using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class AudioAssetControlHandler : AbstractAssetControlHandler<AudioAsset, AudioAssetControl>
	{
		public AudioAssetControlHandler(AudioAsset asset, AudioAssetControl parent)
			: base(asset, parent, "Audio files (*.wav)|*.wav")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
			parent.TextBoxLoudness.Text = Asset.Loudness.ToString();
		}
	}
}