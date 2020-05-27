using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ParticleAssetRowControl : UserControl
	{
		public ParticleAssetRowControlHandler Handler { get; private set; }

		public ParticleAssetRowControl(ParticleAsset asset, bool isEven)
		{
			InitializeComponent();
			TextBlockTags.Text = string.Join(", ", asset.Tags).TrimRight(EditorUtils.TagsMaxLength);

			Handler = new ParticleAssetRowControlHandler(asset, this, TextBlockTags, isEven);

			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();
		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => Handler.UpdateGui();
	}

	public class ParticleAssetRowControlHandler : AbstractAssetRowControlHandler<ParticleAsset, ParticleAssetRowControl>
	{
		public ParticleAssetRowControlHandler(ParticleAsset asset, ParticleAssetRowControl parent, TextBlock textBlockTags, bool isEven)
			: base(asset, parent, "Particle files (*.bin)|*.bin", textBlockTags, isEven)
		{
		}

		public override void UpdateGui()
		{
			parent.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			parent.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : Utils.FileNotFound;
		}
	}
}