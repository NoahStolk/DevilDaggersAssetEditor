using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using SyntaxHighlighter;
using SyntaxHighlighter.Parsers;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ShColor = SyntaxHighlighter.Color;
using WmColor = System.Windows.Media.Color;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
{
	public partial class ShaderPreviewerControl : UserControl, IPreviewerControl
	{
		public ShaderPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(AbstractAsset asset)
		{
			if (asset is not ShaderAsset shaderAsset)
				return;

			ShaderName.Content = shaderAsset.AssetName;

			bool isVertexPathValid = File.Exists(shaderAsset.EditorPath);
			bool isFragmentPathValid = File.Exists(shaderAsset.EditorPathFragmentShader);

			VertexFileName.Content = isVertexPathValid ? Path.GetFileName(shaderAsset.EditorPath) : GuiUtils.FileNotFound;
			FragmentFileName.Content = isFragmentPathValid ? Path.GetFileName(shaderAsset.EditorPathFragmentShader) : GuiUtils.FileNotFound;

			PreviewVertexTextBox.Inlines.Clear();
			PreviewFragmentTextBox.Inlines.Clear();

			if (isVertexPathValid)
			{
				string vertexCode = SanitizeCode(File.ReadAllText(shaderAsset.EditorPath));
				foreach (Piece piece in GlslParser.Instance.Parse(vertexCode))
					PreviewVertexTextBox.Inlines.Add(new Run(piece.Code) { Foreground = new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])) });
			}

			if (isFragmentPathValid)
			{
				string fragmentCode = SanitizeCode(File.ReadAllText(shaderAsset.EditorPathFragmentShader));
				foreach (Piece piece in GlslParser.Instance.Parse(fragmentCode))
					PreviewFragmentTextBox.Inlines.Add(new Run(piece.Code) { Foreground = new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])) });
			}
		}

		// TODO: Improve performance.
		private static string SanitizeCode(string code)
		{
			return code
				.Replace("\r", string.Empty, StringComparison.InvariantCulture)
				.Replace("\n", "\r\n", StringComparison.InvariantCulture)
				.Replace("\t", "    ", StringComparison.InvariantCulture);
		}

		private static WmColor TranslateColor(ShColor color)
			=> WmColor.FromArgb(255, color.R, color.G, color.B);
	}
}
