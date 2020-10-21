using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using DevilDaggersAssetEditor.Wpf.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls
{
	public partial class ShaderAssetRowControl : UserControl
	{
		public ShaderAssetRowControl(AssetRowControlHandler handler)
		{
			Handler = handler;

			InitializeComponent();

			Data.Children.Add(Handler.TextBlockTags);
			Data.Children.Add(Handler.RectangleInfo);
			Data.Children.Add(Handler.RectangleEdit);

			Data.DataContext = Handler.Asset;

			TextBlockVertexName.Text = $"{Handler.Asset.AssetName}_vertex";
			TextBlockFragmentName.Text = $"{Handler.Asset.AssetName}_fragment";

			// Update GUI from row control handlers
			TextBlockVertexEditorPath.Text = File.Exists(Handler.Asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture)) ? Handler.Asset.EditorPath.Insert(Handler.Asset.EditorPath.LastIndexOf('.'), "_vertex").TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
			TextBlockFragmentEditorPath.Text = File.Exists(Handler.Asset.EditorPath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture)) ? Handler.Asset.EditorPath.Insert(Handler.Asset.EditorPath.LastIndexOf('.'), "_fragment").TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
		}

		public AssetRowControlHandler Handler { get; }

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e)
			=> Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
			=> Handler.BrowsePath();

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> Handler.UpdateGui();
	}
}