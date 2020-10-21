using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class ShadersAssetTabControlHandler : AbstractAssetTabControlHandler<ShaderAssetRowControlHandler>
	{
		public ShadersAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType, AssetType.Shader)
		{
		}

		protected override string AssetTypeJsonFileName => "Shaders";
	}
}