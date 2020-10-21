using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersCore.Extensions;
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
	public partial class ShaderPreviewerControl : UserControl
	{
		public ShaderPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(AbstractAsset a)
		{
			ShaderAsset asset = a as ShaderAsset;

			ShaderName.Content = asset.AssetName;

			string vertexPath = asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture);
			bool isPathValid = File.Exists(vertexPath);

			string basePath = isPathValid ? Path.GetFileName(vertexPath).TrimEnd("_vertex") : GuiUtils.FileNotFound;

			VertexFileName.Content = isPathValid ? basePath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture) : basePath;
			FragmentFileName.Content = isPathValid ? basePath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture) : basePath;

			if (isPathValid)
			{
				string vertexCode = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture)));
				string fragmentCode = SanitizeCode(File.ReadAllText(asset.EditorPath.Replace(".glsl", "_fragment.glsl", StringComparison.InvariantCulture)));

				PreviewVertexTextBox.Inlines.Clear();
				foreach (Piece piece in GlslParser.Instance.Parse(vertexCode))
					PreviewVertexTextBox.Inlines.Add(new Run(piece.Code) { Foreground = new SolidColorBrush(TranslateColor(GlslParser.Instance.CodeStyle.HighlightColors[piece.Type])) });

				PreviewFragmentTextBox.Inlines.Clear();
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