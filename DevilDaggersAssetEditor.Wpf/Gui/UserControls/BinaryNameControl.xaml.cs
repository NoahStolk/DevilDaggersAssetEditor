using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class BinaryNameControl : UserControl
	{
		private string _binaryName = string.Empty;

		public BinaryNameControl(BinaryFileType binaryFileType, AssetType assetTypeForColor)
		{
			InitializeComponent();

			BinaryFileType = binaryFileType;

			Progress = new(
				new(value => App.Instance.Dispatcher.Invoke(() => ProgressDescription.Text = value)),
				new(value => App.Instance.Dispatcher.Invoke(() => ProgressBar.Value = value)));

			ProgressBar.Foreground = new SolidColorBrush(EditorUtils.FromRgbTuple(assetTypeForColor.GetColor()) * 0.25f);

			UpdateGui();
		}

		public BinaryFileType BinaryFileType { get; }

		public ProgressWrapper Progress { get; }

		public string? OutputPath { get; private set; }

		public string BinaryName => _binaryName;

		private void TextBoxName_TextChanged(object sender, RoutedEventArgs e)
		{
			_binaryName = TextBoxName.Text;

			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			bool isInvalid = string.IsNullOrWhiteSpace(_binaryName) || _binaryName.Any(c => invalidFileNameChars.Contains(c));
			OutputPath = isInvalid ? null : Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods", $"{BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}_{BinaryName}");
			TextBlockOutputPath.Text = OutputPath;
		}

		private void UpdateGui()
			=> TextBoxName.Text = _binaryName;

		private void CheckBoxEnable_Changed(object sender, RoutedEventArgs e)
		{
			bool isChecked = CheckBoxEnable.IsChecked();

			if (TextBoxName != null)
				TextBoxName.IsEnabled = isChecked;

			Main.Background = ColorUtils.ThemeColors[isChecked ? "Gray3" : "Gray2"];
		}
	}
}
