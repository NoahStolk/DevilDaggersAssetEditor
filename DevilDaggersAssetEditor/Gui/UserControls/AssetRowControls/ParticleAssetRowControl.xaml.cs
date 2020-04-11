using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ParticleAssetRowControl : UserControl
	{
		internal ParticleAssetRowControlHandler Handler { get; private set; }

		public ParticleAssetRowControl(ParticleAsset asset)
		{
			InitializeComponent();

			Handler = new ParticleAssetRowControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class ParticleAssetRowControlHandler : AbstractAssetRowControlHandler<ParticleAsset, ParticleAssetRowControl>
	{
		internal ParticleAssetRowControlHandler(ParticleAsset asset, ParticleAssetRowControl parent)
			: base(asset, parent, "Particle files (*.bin)|*.bin")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}