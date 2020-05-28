using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.IO;

namespace DevilDaggersAssetEditor.Code.RowControlHandlers
{
	public class AudioAssetRowControlHandler : AbstractAssetRowControlHandler<AudioAsset, AudioAssetRowControl>
	{
		public override string OpenDialogFilter => "Audio files (*.wav)|*.wav";

		public AudioAssetRowControlHandler(AudioAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
			AssetRowControl.TextBoxLoudness.Text = Asset.Loudness.ToString();
		}
	}
}