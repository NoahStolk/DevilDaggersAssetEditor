using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ShaderAssetRowControl : UserControl
	{
		public ShaderAssetRowControlHandler Handler { get; private set; }

		public ShaderAssetRowControl(ShaderAssetRowControlHandler handler, bool isEven)
		{
			Handler = handler;

			InitializeComponent();

			Data.Children.Add(Handler.TextBlockTags);
			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = Handler.Asset;

			TextBlockVertexName.Text = $"{Handler.Asset.AssetName}_vertex";
			TextBlockFragmentName.Text = $"{Handler.Asset.AssetName}_fragment";
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => Handler.UpdateGui();
	}

	public class ShaderAssetRowControlHandler : AbstractAssetRowControlHandler<ShaderAsset, ShaderAssetRowControl>
	{
		public override string OpenDialogFilter => "Shader files (*.glsl)|*.glsl";

		public ShaderAssetRowControlHandler(ShaderAsset asset, bool isEven)
			: base(asset, isEven)
		{
		}

		public override void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockVertexEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_vertex.glsl")) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_vertex").TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
			AssetRowControl.TextBlockFragmentEditorPath.Text = File.Exists(Asset.EditorPath.Replace(".glsl", "_fragment.glsl")) ? Asset.EditorPath.Insert(Asset.EditorPath.LastIndexOf('.'), "_fragment").TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
		}
	}
}