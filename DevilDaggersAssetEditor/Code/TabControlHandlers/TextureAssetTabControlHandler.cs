using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class TexturesAssetTabControlHandler : AbstractAssetTabControlHandler<TextureAsset, TextureAssetRowControl, TextureAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Textures";

		public TexturesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(TextureAsset asset)
		{
			TextureAssetRowControl arc = AssetRowEntries.FirstOrDefault(a => a.AssetRowControlHandler.Asset == asset).AssetRowControlHandler.AssetRowControl;
			arc.Handler.UpdateGui();
		}
	}
}