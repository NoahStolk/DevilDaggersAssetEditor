using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersCore.Extensions;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ShaderPreviewerControl : UserControl
	{
		public ShaderPreviewerControl()
		{
			InitializeComponent();

			PreviewVertexTextBox.Document.LineHeight = 2;
			PreviewFragmentTextBox.Document.LineHeight = 2;

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

			PreviewVertexTextBox.Document.Blocks.Clear();
			PreviewFragmentTextBox.Document.Blocks.Clear();

			if (isPathValid)
			{
				string vertexCode = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl")));
				string fragmentCode = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl")));

				PreviewVertexTextBox.Document.PageWidth = MeasureText(vertexCode);
				PreviewFragmentTextBox.Document.PageWidth = MeasureText(fragmentCode);

				new TextRange(PreviewVertexTextBox.Document.ContentEnd, PreviewVertexTextBox.Document.ContentEnd) { Text = vertexCode };
				new TextRange(PreviewFragmentTextBox.Document.ContentEnd, PreviewFragmentTextBox.Document.ContentEnd) { Text = fragmentCode };
			}
		}

		/// <summary>
		/// Provides a hacky way to calculate the width of the shader preview document.
		/// </summary>
		private static double MeasureText(string text)
		{
			int longestLineLength = text.Split('\n').Max(l => l.Length);

			return (longestLineLength + 1) * 6.66f; // Char width
		}

		private static string SanitizeCode(string code) => code.Replace("\t", new string(' ', 4)).Replace("\r", "").Replace("\n", "\r\n");
	}
}