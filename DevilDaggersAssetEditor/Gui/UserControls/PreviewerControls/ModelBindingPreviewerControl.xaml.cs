using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ModelBindingPreviewerControl : UserControl
	{
		public ModelBindingPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ModelBindingAsset asset)
		{
			ModelBindingName.Text = asset.AssetName;

			bool isPathValid = asset.EditorPath.IsPathValid();

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : asset.EditorPath;

			if (isPathValid)
			{
				PreviewTextBox.Text = File.ReadAllText(asset.EditorPath);
			}
		}
	}
}