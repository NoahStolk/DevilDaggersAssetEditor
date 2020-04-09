using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using SyntaxHighlighter;
using SyntaxHighlighter.Parsers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ShColor = SyntaxHighlighter.Color;
using WmColor = System.Windows.Media.Color;

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

			bool isPathValid = asset.EditorPath.IsPathValid();

			string basePath = isPathValid ? Path.GetFileName(asset.EditorPath) : asset.EditorPath;

			VertexFileName.Text = isPathValid ? basePath.Replace(".glsl", "_vertex.glsl") : asset.EditorPath;
			FragmentFileName.Text = isPathValid ? basePath.Replace(".glsl", "_fragment.glsl") : asset.EditorPath;

			if (isPathValid)
			{
				PreviewVertexTextBox.Document.Blocks.Clear();
				PreviewFragmentTextBox.Document.Blocks.Clear();

				string vertexCode = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl")));
				string fragmentCode = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl")));

				PreviewVertexTextBox.Document.PageWidth = MeasureText(vertexCode);
				PreviewFragmentTextBox.Document.PageWidth = MeasureText(fragmentCode);

				List<Piece> parsedVertexCode = GlslParser.Instance.Parse(vertexCode);
				foreach (Piece piece in parsedVertexCode)
				{
					TextRange vertex = new TextRange(PreviewVertexTextBox.Document.ContentEnd, PreviewVertexTextBox.Document.ContentEnd)
					{
						Text = piece.Code
					};
					vertex.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])));
				}

				List<Piece> parsedFragmentCode = GlslParser.Instance.Parse(fragmentCode);
				foreach (Piece piece in parsedFragmentCode)
				{
					TextRange fragment = new TextRange(PreviewFragmentTextBox.Document.ContentEnd, PreviewFragmentTextBox.Document.ContentEnd)
					{
						Text = piece.Code
					};
					fragment.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])));
				}
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

		private static WmColor TranslateColor(ShColor color) => WmColor.FromArgb(255, color.R, color.G, color.B);
	}
}