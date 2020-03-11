using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class TextureAssetControlHandler : AbstractAssetControlHandler<TextureAsset, TextureAssetControl>
	{
		public TextureAssetControlHandler(TextureAsset asset, TextureAssetControl parent)
			: base(asset, parent, "Texture files (*.png)|*.png")
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}