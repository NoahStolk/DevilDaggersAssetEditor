using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.PreviewerControls
{
	public partial class ParticlePreviewerControl : UserControl
	{
		public ParticlePreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ParticleAsset asset)
		{
			ParticleName.Text = asset.AssetName;

			bool isPathValid = asset.EditorPath.IsPathValid();

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : asset.EditorPath;

			if (isPathValid)
			{
				PreviewTextBlock.Text = Encoding.UTF8.GetString(File.ReadAllBytes(asset.EditorPath));
			}
		}
	}
}