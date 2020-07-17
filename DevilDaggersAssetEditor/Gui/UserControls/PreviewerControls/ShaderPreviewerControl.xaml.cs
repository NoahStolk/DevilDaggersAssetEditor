using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersCore.Extensions;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ShaderPreviewerControl : UserControl
	{
		public ShaderPreviewerControl()
		{
			InitializeComponent();

			PreviewVertexTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
			PreviewFragmentTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
		}

		public void Initialize(ShaderAsset asset)
		{
			ShaderName.Text = asset.AssetName;

			string vertexPath = asset.EditorPath.Replace(".glsl", "_vertex.glsl");
			bool isPathValid = File.Exists(vertexPath);

			string basePath = isPathValid ? Path.GetFileName(vertexPath).TrimEnd("_vertex") : Utils.FileNotFound;

			VertexFileName.Text = isPathValid ? basePath.Replace(".glsl", "_vertex.glsl") : basePath;
			FragmentFileName.Text = isPathValid ? basePath.Replace(".glsl", "_fragment.glsl") : basePath;

			PreviewVertexTextBox.Clear();
			PreviewFragmentTextBox.Clear();

			if (isPathValid)
			{
				PreviewVertexTextBox.Text = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl")));
				PreviewFragmentTextBox.Text = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl")));
			}
		}

		private static string SanitizeCode(string code) => code.Replace("\t", new string(' ', 4)).Replace("\r", "").Replace("\n", "\r\n");
	}
}