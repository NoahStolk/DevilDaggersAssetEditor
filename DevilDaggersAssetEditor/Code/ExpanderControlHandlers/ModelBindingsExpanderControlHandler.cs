using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.ExpanderControlHandlers
{
	public class ModelBindingsExpanderControlHandler : AbstractExpanderControlHandler<ModelBindingAsset, ModelBindingAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Model Bindings";

		public ModelBindingsExpanderControlHandler(BinaryFileType binaryFileType)
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