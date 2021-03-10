﻿using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
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
		private readonly MakeBinariesWindow? _parent;

		private string _binaryName = string.Empty;

		public BinaryNameControl(MakeBinariesWindow? parent, BinaryFileType binaryFileType, AssetType assetTypeForColor, bool checkBoxIsChecked)
		{
			InitializeComponent();

			_parent = parent;

			BinaryFileType = binaryFileType;

			LabelModFileName.Content = $"'{binaryFileType.ToString().ToLower(CultureInfo.InvariantCulture)}' mod file name";

			Progress = new(
				new(value => App.Instance.Dispatcher.Invoke(() => ProgressDescription.Text = value)),
				new(value => App.Instance.Dispatcher.Invoke(() => ProgressBar.Value = value)));

			ProgressBar.Foreground = new SolidColorBrush(EditorUtils.FromRgbTuple(assetTypeForColor.GetColor()) * 0.25f);

			CheckBoxEnable.IsChecked = checkBoxIsChecked;

			UpdateGui();
		}

		public BinaryFileType BinaryFileType { get; }

		public ProgressWrapper Progress { get; }

		public string? OutputPath { get; private set; }

		public string BinaryName => _binaryName;

		private void TextBoxName_TextChanged(object sender, RoutedEventArgs e)
		{
			UpdateName(TextBoxName.Text);

			_parent?.UpdateButtonMakeBinaries();
		}

		public void UpdateName(string name)
		{
			_binaryName = name;

			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			bool isInvalid = string.IsNullOrWhiteSpace(_binaryName) || _binaryName.Any(c => invalidFileNameChars.Contains(c));
			OutputPath = isInvalid ? null : Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods", BinaryFileType.ToString().ToLower(CultureInfo.InvariantCulture) + BinaryName);
			TextBlockOutputPath.Text = OutputPath;
		}

		public void UpdateGui()
			=> TextBoxName.Text = _binaryName;

		private void CheckBoxEnable_Changed(object sender, RoutedEventArgs e)
		{
			bool isChecked = CheckBoxEnable.IsChecked();

			if (TextBoxName != null)
				TextBoxName.IsEnabled = isChecked;

			Main.Background = ColorUtils.ThemeColors[isChecked ? "Gray3" : "Gray2"];

			_parent?.UpdateButtonMakeBinaries();
		}
	}
}