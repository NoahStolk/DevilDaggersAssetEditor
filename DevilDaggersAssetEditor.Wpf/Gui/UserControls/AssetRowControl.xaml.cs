using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersAssetEditor.Wpf.ModFiles;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Mods;
using DevilDaggersCore.Wpf.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
			Grid.SetColumnSpan(RectangleEdit, 4);
			Grid.SetRowSpan(RectangleEdit, 2);

			UpdateBackgroundRectangleColors(isEven);

			Data.Children.Add(RectangleInfo);
			Data.Children.Add(RectangleEdit);

			TextBlockAssetName.Text = Asset.AssetName;

			if (Asset is ShaderAsset shaderAsset)
			{
				ShaderAsset = shaderAsset;
				Height = 40;
			}
			else if (Asset is AudioAsset audioAsset)
			{
				AudioAsset = audioAsset;
				ColumnDefinitionLoudness.Width = new(1, GridUnitType.Star);
				ColumnDefinitionPath.Width = new(5, GridUnitType.Star);
				TextBoxLoudness.Visibility = Visibility.Visible;
			}
		}

		public Rectangle RectangleInfo { get; } = new();
		public Rectangle RectangleEdit { get; } = new();

		public AbstractAsset Asset { get; }
		public AudioAsset? AudioAsset { get; }
		public ShaderAsset? ShaderAsset { get; }
		public AssetType AssetType { get; }

		public bool IsActive { get; set; } = true;

		public string OpenDialogFilter { get; }

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e)
		{
			Asset.EditorPath = GuiUtils.FileNotFound;
			if (ShaderAsset != null)
				ShaderAsset.EditorPathFragmentShader = GuiUtils.FileNotFound;

			UpdateGui();

			ModFileHandler.Instance.HasUnsavedChanges = true;
		}

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
		{
			if (ShaderAsset != null)
			{
				SetShaderPathsWindow window = new(OpenDialogFilter, Asset.AssetName);
				if (window.ShowDialog() == true)
				{
					ShaderAsset.EditorPath = window.VertexPath!;
					ShaderAsset.EditorPathFragmentShader = window.FragmentPath!;
				}
			}
			else
			{
				OpenFileDialog openDialog = new() { Filter = OpenDialogFilter, Title = $"Select source for {AssetType.ToString().ToLower()} '{Asset.AssetName}'" };
				openDialog.OpenDirectory(UserHandler.Instance.Settings.EnableAssetsRootFolder, UserHandler.Instance.Settings.AssetsRootFolder);

				bool? openResult = openDialog.ShowDialog();
				if (!openResult.HasValue || !openResult.Value)
					return;

				Asset.EditorPath = openDialog.FileName;
			}

			UpdateGui();

			foreach (AssetTabControl tabControl in App.Instance.MainWindow!.AssetTabControls)
			{
				if (tabControl.SelectedAsset == Asset && tabControl.Previewer is IPreviewerControl previewer)
					previewer.Initialize(Asset);
			}

			ModFileHandler.Instance.HasUnsavedChanges = true;
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> UpdateGui();

		public void UpdateBackgroundRectangleColors(bool isEven)
		{
			RectangleInfo.Fill = isEven ? _brushInfoEven : _brushInfoOdd;
			RectangleEdit.Fill = isEven ? _brushEditEven : _brushEditOdd;
		}

		public void UpdateGui()
		{
			TextBlockDescription.Text = Asset.Description ?? "Not fetched";

			string editorPath = File.Exists(Asset.EditorPath) ? Asset.EditorPath : GuiUtils.FileNotFound;
			if (ShaderAsset != null)
			{
				string fragmentEditorPath = File.Exists(ShaderAsset.EditorPathFragmentShader) ? ShaderAsset.EditorPathFragmentShader : GuiUtils.FileNotFound;
				TextBlockEditorPath.Text = $"{editorPath}\n{fragmentEditorPath}";
			}
			else
			{
				TextBlockEditorPath.Text = editorPath;
			}

			if (AudioAsset != null)
				TextBoxLoudness.Text = AudioAsset.Loudness.ToString();
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

		private void TextBoxLoudness_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AudioAsset == null)
				return;

			bool isValid = float.TryParse(TextBoxLoudness.Text, out float loudness) && loudness >= 0;

			TextBoxLoudness.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			if (isValid)
			{
				AudioAsset.Loudness = loudness;
				ModFileHandler.Instance.HasUnsavedChanges = App.Instance.MainWindow!.HasLoaded;
			}
		}
	}
}
