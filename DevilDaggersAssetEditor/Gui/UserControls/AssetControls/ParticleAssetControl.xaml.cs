using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetControls
{
	public partial class ParticleAssetControl : UserControl
	{
		internal ParticleAssetControlHandler Handler { get; private set; }

		public ParticleAssetControl(ParticleAsset asset)
		{
			InitializeComponent();

			Handler = new ParticleAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	internal class ParticleAssetControlHandler : AbstractAssetControlHandler<ParticleAsset, ParticleAssetControl>
	{
		internal ParticleAssetControlHandler(ParticleAsset asset, ParticleAssetControl parent)
			: base(asset, parent, "Particle files (*.bin)|*.bin")
		{
		}

		internal override void UpdateGui()
		{
			parent.TextBlockEditorPath.Text = Asset.EditorPath;
		}
	}
}