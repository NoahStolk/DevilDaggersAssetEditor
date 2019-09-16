﻿using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class AudioAssetControlHandler : AbstractAssetControlHandler<AudioAsset, AudioAssetControl>
	{
		public AudioAssetControlHandler(AudioAsset asset, AudioAssetControl parent)
			: base(asset, parent, "Audio files (*.wav)|*.wav")
		{
		}

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}