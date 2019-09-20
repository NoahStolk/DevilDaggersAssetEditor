using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.ExpanderControlHandlers
{
	public class TexturesExpanderControlHandler : AbstractExpanderControlHandler<TextureAsset, TextureAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Textures";

		public TexturesExpanderControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGUI(TextureAsset asset)
		{
			TextureAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}