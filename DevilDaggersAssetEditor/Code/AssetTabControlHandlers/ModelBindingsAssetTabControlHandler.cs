using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.Previewers;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.AssetTabControlHandlers
{
	public class ModelBindingsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelBindingAsset, ModelBindingAssetControl, ModelBindingPreviewer>
	{
		protected override string AssetTypeJsonFileName => "Model Bindings";

		public ModelBindingsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGUI(ModelBindingAsset asset)
		{
			ModelBindingAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}