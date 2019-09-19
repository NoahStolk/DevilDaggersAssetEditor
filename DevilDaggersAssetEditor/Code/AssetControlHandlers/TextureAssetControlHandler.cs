using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public class TextureAssetControlHandler : AbstractAssetControlHandler<TextureAsset, TextureAssetControl>
	{
		public TextureAssetControlHandler(TextureAsset asset, TextureAssetControl parent)
			: base(asset, parent, "Texture files (*.png)|*.png")
		{
		}

		public override void UpdateGUI()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}