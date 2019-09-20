﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ModelsTabControlHandler : AbstractTabControlHandler<ModelAsset, ModelAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Models";

		public ModelsTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGUI(ModelAsset asset)
		{
			ModelAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}