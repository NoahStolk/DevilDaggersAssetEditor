using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ModelPreviewerControl : UserControl
	{
		public ModelPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ModelAsset asset)
		{
			TextureName.Text = asset.AssetName;
			DefaultVertexCount.Text = asset.DefaultVertexCount.ToString();
			DefaultIndexCount.Text = asset.DefaultIndexCount.ToString();

			bool isPathValid = asset.EditorPath.IsPathValid();

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : asset.EditorPath;

			if (isPathValid)
			{
				// TODO: Open in OBJ Viewer
			}
		}
	}
}