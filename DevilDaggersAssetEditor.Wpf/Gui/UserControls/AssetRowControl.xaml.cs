using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;
using DevilDaggersAssetEditor.Wpf.ModFiles;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class AssetRowControl : UserControl
	{
		private const string _tagSeparator = ", ";

		private readonly SolidColorBrush _brushInfoEven;
		private readonly SolidColorBrush _brushInfoOdd;
		private readonly SolidColorBrush _brushEditEven;
		private readonly SolidColorBrush _brushEditOdd;

		private readonly ShaderAsset? _shaderAsset;
		private readonly AudioAsset? _audioAsset;

		public AssetRowControl(AbstractAsset asset, AssetType assetType, bool isEven, string openDialogFilter)
		{
			InitializeComponent();

			OpenDialogFilter = openDialogFilter;

			Asset = asset;
			AssetType = assetType;
			TextBlockProhibited.Text = asset.IsProhibited ? "Yes" : "No";
			TextBlockTags.Text = string.Join(_tagSeparator, asset.Tags);

			Color colorEditEven = EditorUtils.FromRgbTuple(assetType.GetColor()) * 0.25f;
			Color colorEditOdd = colorEditEven * 0.5f;
			Color colorInfoEven = colorEditOdd;
			Color colorInfoOdd = colorEditOdd * 0.5f;
			_brushInfoEven = new(colorInfoEven);
			_brushInfoOdd = new(colorInfoOdd);
			_brushEditEven = new(colorEditEven);
			_brushEditOdd = new(colorEditOdd);

			Panel.SetZIndex(RectangleInfo, -1);
			Grid.SetColumnSpan(RectangleInfo, 4);
			Grid.SetRowSpan(RectangleInfo, 2);

			Panel.SetZIndex(RectangleEdit, -1);
			Grid.SetColumn(RectangleEdit, 4);
			Grid.SetColumnSpan(RectangleEdit, 3);
			Grid.SetRowSpan(RectangleEdit, 2);

			UpdateBackgroundRectangleColors(isEven);

			Data.Children.Add(RectangleInfo);
			Data.Children.Add(RectangleEdit);

			TextBlockAssetName.Text = Asset.AssetName;

			if (Asset is ShaderAsset shaderAsset)
			{
				_shaderAsset = shaderAsset;
				BrowseButtonFragmentShader.Visibility = Visibility.Visible;
				RemovePathButtonFragmentShader.Visibility = Visibility.Visible;
				TextBlockEditorPathFragmentShader.Visibility = Visibility.Visible;
				RowDefinitionFragmentShader.Height = new(24);
				Grid.SetRowSpan(TextBlockTags, 2);
			}

			if (Asset is AudioAsset audioAsset)
			{
				_audioAsset = audioAsset;
				ColumnDefinitionLoudness.Width = new(96);
				TextBoxLoudness.Visibility = Visibility.Visible;
			}
		}

		public Rectangle RectangleInfo { get; } = new();
		public Rectangle RectangleEdit { get; } = new();

		public AbstractAsset Asset { get; }
		public AssetType AssetType { get; }

		public bool IsActive { get; set; } = true;

		public string OpenDialogFilter { get; }

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e)
			=> RemovePath(false);

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
			=> BrowsePath(false);

		private void ButtonRemovePathFragmentShader_Click(object sender, RoutedEventArgs e)
			=> RemovePath(true);

		private void ButtonBrowsePathFragmentShader_Click(object sender, RoutedEventArgs e)
			=> BrowsePath(true);

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> UpdateGui();

		public void UpdateBackgroundRectangleColors(bool isEven)
		{
			RectangleInfo.Fill = isEven ? _brushInfoEven : _brushInfoOdd;
			RectangleEdit.Fill = isEven ? _brushEditEven : _brushEditOdd;
		}

		public void UpdateGui()
		{
			TextBlockDescription.Text = Asset.Description;
			TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath : GuiUtils.FileNotFound;
			if (_shaderAsset != null)
				TextBlockEditorPathFragmentShader.Text = File.Exists(_shaderAsset.EditorPathFragmentShader) ? _shaderAsset.EditorPathFragmentShader : GuiUtils.FileNotFound;
			if (_audioAsset != null)
				TextBoxLoudness.Text = _audioAsset.Loudness.ToString();
		}

		public void UpdateTagHighlighting(IEnumerable<string> checkedFilters, Color filterHighlightColor)
		{
			if (!checkedFilters.Any())
			{
				TextBlockTags.Text = string.Join(_tagSeparator, Asset.Tags);
				return;
			}

			TextBlockTags.Inlines.Clear();

			for (int i = 0; i < Asset.Tags.Count; i++)
			{
				string tag = Asset.Tags[i];
				Run tagRun = new(tag);
				if (checkedFilters.Contains(tag))
					tagRun.Background = new SolidColorBrush(filterHighlightColor);

				TextBlockTags.Inlines.Add(tagRun);
				if (i != Asset.Tags.Count - 1)
					TextBlockTags.Inlines.Add(new Run(_tagSeparator));
			}
		}

		public void BrowsePath(bool fragmentShader)
		{
			OpenFileDialog openDialog = new() { Filter = OpenDialogFilter };
			openDialog.OpenDirectory(UserHandler.Instance.Settings.EnableAssetsRootFolder, UserHandler.Instance.Settings.AssetsRootFolder);

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			SetPath(fragmentShader, openDialog.FileName);
			UpdateGui();

			if (_audioAsset != null && App.Instance.MainWindow!.AudioAudioAssetTabControl.SelectedAsset == Asset && App.Instance.MainWindow!.AudioAudioAssetTabControl.Previewer is IPreviewerControl audioPreviewer)
				audioPreviewer.Initialize(Asset);

			ModFileHandler.Instance.HasUnsavedChanges = true;
		}

		public void RemovePath(bool fragmentShader)
		{
			SetPath(fragmentShader, GuiUtils.FileNotFound);
			UpdateGui();

			ModFileHandler.Instance.HasUnsavedChanges = true;
		}

		// TODO: Use this method to set paths on initial file load (from cache).
		public void SetPath(bool fragmentShader, string path)
		{
			if (fragmentShader && _shaderAsset != null)
				_shaderAsset.EditorPathFragmentShader = path;
			else
				Asset.EditorPath = path;
		}

		private void TextBoxLoudness_TextChanged(object sender, TextChangedEventArgs e)
		{
			Debug.Assert(_audioAsset != null, $"Should not be able to edit {nameof(TextBoxLoudness)} when asset is not an audio asset.");
			bool isValid = float.TryParse(TextBoxLoudness.Text, out float loudness) && loudness >= 0;

			TextBoxLoudness.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			if (isValid)
			{
				_audioAsset.Loudness = loudness;
				ModFileHandler.Instance.HasUnsavedChanges = App.Instance.MainWindow!.HasLoaded;
			}
		}
	}
}
