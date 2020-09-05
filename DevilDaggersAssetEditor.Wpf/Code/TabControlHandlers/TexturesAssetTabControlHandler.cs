using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Wpf.Code.TabControlHandlers
{
	public class TexturesAssetTabControlHandler : AbstractAssetTabControlHandler<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>
	{
		public TexturesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		protected override string AssetTypeJsonFileName => "Textures";
	}
}