using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
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
		private string _binaryPath;

		public BinaryPathControl(string header, BinaryFileType binaryFileType, AssetType assetTypeForColor)
		{
			InitializeComponent();

			BinaryFileType = binaryFileType;

			Header.Content = header;

			Progress = new ProgressWrapper(
				new Progress<string>(value => App.Instance.Dispatcher.Invoke(() => ProgressDescription.Text = value)),
				new Progress<float>(value => App.Instance.Dispatcher.Invoke(() => ProgressBar.Value = value)));

			ProgressBar.Foreground = new SolidColorBrush(EditorUtils.FromRgbTuple(assetTypeForColor.GetColor()) * 0.25f);

			_binaryPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, binaryFileType.GetSubfolderName(), binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture));
			UpdateGui();
		}

		public BinaryFileType BinaryFileType { get; }

		public ProgressWrapper Progress { get; }

		public string BinaryPath => _binaryPath;

		private void TextBoxPath_TextChanged(object sender, RoutedEventArgs e)
			=> _binaryPath = TextBoxPath.Text;

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
			=> SetPath(ref _binaryPath);

		private void UpdateGui()
			=> TextBoxPath.Text = _binaryPath;

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
			openDialog.OpenDirectory(UserHandler.Instance.Settings.EnableDevilDaggersRootFolder, Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, BinaryFileType.GetSubfolderName()));

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
			{
				selectedPath = string.Empty;
				return false;
			}

			selectedPath = openDialog.FileName;
			return true;
		}

		private void CheckBoxEnable_Changed(object sender, RoutedEventArgs e)
		{
			bool isChecked = CheckBoxEnable.IsChecked();

			if (TextBoxPath != null)
				TextBoxPath.IsEnabled = isChecked;
			if (ButtonBrowse != null)
				ButtonBrowse.IsEnabled = isChecked;

			Main.Background = ColorUtils.ThemeColors[isChecked ? "Gray3" : "Gray2"];
		}
	}
}