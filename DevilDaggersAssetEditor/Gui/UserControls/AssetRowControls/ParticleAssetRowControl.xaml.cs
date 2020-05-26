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
			TextBlockTags.Text = asset.Tags != null ? string.Join(", ", asset.Tags) : string.Empty;

			Handler = new ParticleAssetRowControlHandler(asset, this, TextBlockTags, isEven);

			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ParticleAssetRowControlHandler : AbstractAssetRowControlHandler<ParticleAsset, ParticleAssetRowControl>
	{
		public ParticleAssetRowControlHandler(ParticleAsset asset, ParticleAssetRowControl parent, TextBlock textBlockTags, bool isEven)
			: base(asset, parent, "Particle files (*.bin)|*.bin", textBlockTags, isEven)
		{
		}

		public override void UpdateGui()
		{
			bool isPathValid = File.Exists(Asset.EditorPath);
			parent.TextBlockEditorPath.Text = isPathValid ? Asset.EditorPath : Utils.FileNotFound;
		}
	}
}