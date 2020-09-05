using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Wpf.Code.TabControlHandlers
{
	public class ShadersAssetTabControlHandler : AbstractAssetTabControlHandler<ShaderAsset, ShaderAssetRowControl, ShaderAssetRowControlHandler>
	{
		public ShadersAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		protected override string AssetTypeJsonFileName => "Shaders";
	}
}