using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class AudioAssetControlHandler : AbstractAssetControlHandler<AudioAsset, AudioAssetControl>
	{
		public AudioAssetControlHandler(AudioAsset asset, AudioAssetControl audioAssetControl)
			: base(asset, audioAssetControl, "Audio files (*.wav)|*.wav")
		{
		}

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.LabelEditorPath.Content = Asset.EditorPath;
		}
	}
}