using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls.PreviewerControls
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

			string basePath = isPathValid ? Path.GetFileNameWithoutExtension(asset.EditorPath) : asset.EditorPath;

			VertexFileName.Text = isPathValid ? basePath.Replace(".glsl", "_vertex.glsl") : asset.EditorPath;
			FragmentFileName.Text = isPathValid ? basePath.Replace(".glsl", "_fragment.glsl") : asset.EditorPath;

			if (isPathValid)
			{
				PreviewVertexTextBlock.Text = File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl"));
				PreviewFragmentTextBlock.Text = File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl"));
			}
		}
	}
}