using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.AssetTabControlHandlers
{
	public class ModelsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelAsset, ModelAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Models";

		public ModelsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(ModelAsset asset)
		{
			ModelAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}