using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class TexturesAssetTabControlHandler : AbstractAssetTabControlHandler<TextureAssetRowControlHandler>
	{
		public TexturesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType, AssetType.Texture)
		{
		}

		protected override string AssetTypeJsonFileName => "Textures";
	}
}