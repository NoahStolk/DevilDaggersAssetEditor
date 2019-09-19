using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ShadersTabControlHandler : AbstractTabControlHandler<ShaderAsset, ShaderAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Shaders";

		public ShadersTabControlHandler(BinaryFileName binaryFileName)
			: base(binaryFileName)
		{
		}

		public override void UpdateGUI(ShaderAsset asset)
		{
			ShaderAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}