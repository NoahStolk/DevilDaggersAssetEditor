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

		protected override void UpdatePathLabel()
		{
			// TODO: Fix binding
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}