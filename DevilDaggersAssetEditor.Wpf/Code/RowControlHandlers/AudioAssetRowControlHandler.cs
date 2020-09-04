using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using System.Globalization;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers
{
	public class AudioAssetRowControlHandler : AbstractAssetRowControlHandler<AudioAsset, AudioAssetRowControl>
	{
		public AudioAssetRowControlHandler(AudioAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override string OpenDialogFilter => "Audio files (*.wav)|*.wav";

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
			AssetRowControl.TextBoxLoudness.Text = Asset.Loudness.ToString(CultureInfo.InvariantCulture);
		}
	}
}