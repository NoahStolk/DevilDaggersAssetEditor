using DevilDaggersAssetEditor.Code.RowControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ModelAssetRowControl : UserControl
	{
		public ModelAssetRowControlHandler Handler { get; }

		public ModelAssetRowControl(ModelAssetRowControlHandler handler)
		{
			Handler = handler;

			InitializeComponent();

			Data.Children.Add(Handler.TextBlockTags);
			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Data.DataContext = Handler.Asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => Handler.UpdateGui();
	}
}