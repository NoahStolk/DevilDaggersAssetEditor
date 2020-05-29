using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ShadersAssetTabControlHandler : AbstractAssetTabControlHandler<ShaderAsset, ShaderAssetRowControl, ShaderAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Shaders";

		public ShadersAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}
	}
}