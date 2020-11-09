using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Utils;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class BinaryPathControl : UserControl
	{
		private readonly BinaryFileType _binaryFileType;

		private string _binaryPath;

		public BinaryPathControl(string header, BinaryFileType binaryFileType, AssetType assetTypeForColor)
		{
			InitializeComponent();

			_binaryFileType = binaryFileType;

			Header.Content = header;

			Progress = new ProgressWrapper(
				new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => ProgressDescription.Text = value)),
				new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => ProgressBar.Value = value)));

			ProgressBar.Foreground = new SolidColorBrush(EditorUtils.FromRgbTuple(assetTypeForColor.GetColor()) * 0.5f);

			_binaryPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, binaryFileType.GetSubfolderName(), binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture));
			UpdateGui();
		}

		public ProgressWrapper Progress { get; }

		public string BinaryPath => _binaryPath;

		private void TextBox_TextChanged(object sender, RoutedEventArgs e)
			=> _binaryPath = TextBox.Text;

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _binaryPath);

		private void UpdateGui()
			=> TextBox.Text = _binaryPath;

		private void SetPath(ref string path)
		{
			if (TrySetPath(out string selectedPath))
			{
				path = selectedPath;
				UpdateGui();
			}
		}

		private bool TrySetPath(out string selectedPath)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.OpenDirectory(UserHandler.Instance.Settings.EnableDevilDaggersRootFolder, Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, _binaryFileType.GetSubfolderName()));

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = openDialog.FileName;
			return true;
		}
	}
}