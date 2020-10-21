using DevilDaggersAssetEditor.Assets;

namespace DevilDaggersAssetEditor.Wpf.RowControlHandlers
{
	public class AudioAssetRowControlHandler : AssetRowControlHandler
	{
		public AudioAssetRowControlHandler(AbstractAsset asset, bool isEven)
			: base(asset, AssetType.Audio, isEven, "Audio files (*.wav)|*.wav")
		{
		}

		public override void UpdateGui()
		{
			base.UpdateGui();

			// AssetRowControl.TextBoxLoudness.Text = Asset.Loudness.ToString(CultureInfo.InvariantCulture);
		}
	}
}