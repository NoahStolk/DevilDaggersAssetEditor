using DevilDaggersAssetEditor.Assets;
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
			if (!(asset is ShaderAsset shaderAsset))
				return;

			ShaderName.Content = shaderAsset.AssetName;

			VertexFileName.Content = shaderAsset.EditorPath;
			FragmentFileName.Content = shaderAsset.EditorPathFragmentShader;

			PreviewVertexTextBox.Inlines.Clear();
			PreviewFragmentTextBox.Inlines.Clear();

			if (File.Exists(shaderAsset.EditorPath))
			{
				string vertexCode = SanitizeCode(File.ReadAllText(shaderAsset.EditorPath));
				foreach (Piece piece in GlslParser.Instance.Parse(vertexCode))
					PreviewVertexTextBox.Inlines.Add(new Run(piece.Code) { Foreground = new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])) });
			}

			if (File.Exists(shaderAsset.EditorPathFragmentShader))
			{
				string fragmentCode = SanitizeCode(File.ReadAllText(shaderAsset.EditorPathFragmentShader));
				foreach (Piece piece in GlslParser.Instance.Parse(fragmentCode))
					PreviewFragmentTextBox.Inlines.Add(new Run(piece.Code) { Foreground = new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])) });
			}
		}

		private static string SanitizeCode(string code)
			=> code.Replace("\t", new string(' ', 4), StringComparison.InvariantCulture).Replace("\r", string.Empty, StringComparison.InvariantCulture).Replace("\n", "\r\n", StringComparison.InvariantCulture);

		private static WmColor TranslateColor(ShColor color)
			=> WmColor.FromArgb(255, color.R, color.G, color.B);
	}
}