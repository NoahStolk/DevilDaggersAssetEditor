using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class TexturesAssetTabControlHandler : AbstractAssetTabControlHandler<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Textures";

		public TexturesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}
	}
}