using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
{
	public partial class ModelBindingPreviewerControl : UserControl
	{
		public ModelBindingPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ModelBindingAsset asset)
		{
			ModelBindingName.Content = asset.AssetName;

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Content = isPathValid ? Path.GetFileName(asset.EditorPath) : GuiUtils.FileNotFound;

			if (isPathValid)
				PreviewTextBox.Text = File.ReadAllText(asset.EditorPath);
			else
				PreviewTextBox.Text = string.Empty;
		}
	}
}