using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class SetShaderPathsWindow : Window
	{
		private string? _vertexPath;
		private string? _fragmentPath;

		private readonly string _openDialogFilter;
		private readonly string _assetName;

		public SetShaderPathsWindow(string openDialogFilter, string assetName)
		{
			InitializeComponent();

			_openDialogFilter = openDialogFilter;
			_assetName = assetName;
		}

		public string? VertexPath => _vertexPath;
		public string? FragmentPath => _fragmentPath;

		private void TextBoxPathVertex_TextChanged(object sender, RoutedEventArgs e)
		{
			_vertexPath = TextBoxPathVertex.Text;
			UpdateButton();
		}

		private void TextBoxPathFragment_TextChanged(object sender, RoutedEventArgs e)
		{
			_fragmentPath = TextBoxPathFragment.Text;
			UpdateButton();
		}

		private void BrowseButtonVertex_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _vertexPath, "vertex");

		private void BrowseButtonFragment_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _fragmentPath, "fragment");

		private void UpdateTextBoxes()
		{
			TextBoxPathVertex.Text = VertexPath;
			TextBoxPathFragment.Text = FragmentPath;
		}

		private void UpdateButton()
		{
			OkButton.IsEnabled = !string.IsNullOrWhiteSpace(VertexPath) && !string.IsNullOrWhiteSpace(FragmentPath) && File.Exists(VertexPath) && File.Exists(FragmentPath);
		}

		private void SetPath(ref string? path, string shaderType)
		{
			if (TrySetPath(out string selectedPath, shaderType))
			{
				path = selectedPath;
				UpdateTextBoxes();
				UpdateButton();
			}
		}

		private bool TrySetPath(out string selectedPath, string shaderType)
		{
			OpenFileDialog openDialog = new() { Filter = _openDialogFilter, Title = $"Select source for {shaderType} shader '{_assetName}'" };
			openDialog.OpenDirectory(UserHandler.Instance.Settings.EnableAssetsRootFolder, UserHandler.Instance.Settings.AssetsRootFolder);

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = openDialog.FileName;
			return true;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_vertexPath) || string.IsNullOrWhiteSpace(_fragmentPath))
				return;

			if (!File.Exists(_vertexPath) || !File.Exists(_fragmentPath))
				return;

			DialogResult = true;
		}
	}
}
