using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ShaderPreviewerControl : UserControl
	{
		public ShaderPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ShaderAsset asset)
		{
			ShaderName.Text = asset.AssetName;

			bool isPathValid = asset.EditorPath.IsPathValid();

			string basePath = isPathValid ? Path.GetFileName(asset.EditorPath) : asset.EditorPath;

			VertexFileName.Text = isPathValid ? basePath.Replace(".glsl", "_vertex.glsl") : asset.EditorPath;
			FragmentFileName.Text = isPathValid ? basePath.Replace(".glsl", "_fragment.glsl") : asset.EditorPath;

			if (isPathValid)
			{
				PreviewVertexTextBox.Text = File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl"));
				PreviewFragmentTextBox.Text = File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl"));
			}
		}
	}
}