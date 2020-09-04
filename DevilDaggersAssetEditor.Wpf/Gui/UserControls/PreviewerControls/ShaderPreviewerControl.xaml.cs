using DevilDaggersAssetEditor.Assets;
using DevilDaggersCore.Extensions;
using System;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
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

			string vertexPath = asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture);
			bool isPathValid = File.Exists(vertexPath);

			string basePath = isPathValid ? Path.GetFileName(vertexPath).TrimEnd("_vertex") : Utils.FileNotFound;

			VertexFileName.Text = isPathValid ? basePath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture) : basePath;
			FragmentFileName.Text = isPathValid ? basePath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture) : basePath;

			PreviewVertexTextBox.Clear();
			PreviewFragmentTextBox.Clear();

			if (isPathValid)
			{
				PreviewVertexTextBox.Text = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture)));
				PreviewFragmentTextBox.Text = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture)));
			}
		}

		private static string SanitizeCode(string code)
			=> code.Replace("\t", new string(' ', 4), StringComparison.InvariantCulture).Replace("\r", string.Empty, StringComparison.InvariantCulture).Replace("\n", "\r\n", StringComparison.InvariantCulture);
	}
}