using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class TexturesTabControlHandler : AbstractTabControlHandler<TextureAsset, TextureAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Textures";

		public TexturesTabControlHandler(BinaryFileName binaryFileName)
			: base(binaryFileName)
		{
		}

		protected override void UpdatePathLabel(TextureAsset asset)
		{
			TextureAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}