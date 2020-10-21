using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Info;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.Utils;
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

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls
{
	public class AssetRowControlHandler
	{
		private readonly SolidColorBrush _brushInfoEven;
		private readonly SolidColorBrush _brushInfoOdd;
		private readonly SolidColorBrush _brushEditEven;
		private readonly SolidColorBrush _brushEditOdd;

		public AssetRowControlHandler(AbstractAsset asset, AssetType assetType, bool isEven, string openDialogFilter)
		{
			OpenDialogFilter = openDialogFilter;

			Asset = asset;
			TextBlockTags = new TextBlock
			{
				Text = string.Join(", ", asset.Tags).TrimRight(EditorUtils.TagsMaxLength),
				Margin = new Thickness(2),
				Foreground = ColorUtils.ThemeColors["Text"],
			};
			Grid.SetColumn(TextBlockTags, 1);

			AssetRowControl = (AssetRowControl)Activator.CreateInstance(typeof(AssetRowControl), this);

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == assetType);
			Color colorEditEven = chunkInfo.GetColor() * 0.25f;
			Color colorEditOdd = colorEditEven * 0.5f;
			Color colorInfoEven = colorEditOdd;
			Color colorInfoOdd = colorEditOdd * 0.5f;
			_brushInfoEven = new SolidColorBrush(colorInfoEven);
			_brushInfoOdd = new SolidColorBrush(colorInfoOdd);
			_brushEditEven = new SolidColorBrush(colorEditEven);
			_brushEditOdd = new SolidColorBrush(colorEditOdd);

			UpdateGui();

			Panel.SetZIndex(RectangleInfo, -1);
			Grid.SetColumnSpan(RectangleInfo, 3);

			Panel.SetZIndex(RectangleEdit, -1);
			Grid.SetColumn(RectangleEdit, 3);
			Grid.SetColumnSpan(RectangleEdit, 4);

			UpdateBackgroundRectangleColors(isEven);
		}

		public Rectangle RectangleInfo { get; } = new Rectangle();
		public Rectangle RectangleEdit { get; } = new Rectangle();

		public AbstractAsset Asset { get; }
		public AssetRowControl AssetRowControl { get; }

		public bool IsActive { get; set; } = true;

		public string OpenDialogFilter { get; }

		public TextBlock TextBlockTags { get; }

		public void UpdateBackgroundRectangleColors(bool isEven)
		{
			RectangleInfo.Fill = isEven ? _brushInfoEven : _brushInfoOdd;
			RectangleEdit.Fill = isEven ? _brushEditEven : _brushEditOdd;
		}

		public virtual void UpdateGui()
		{
			AssetRowControl.TextBlockDescription.Text = Asset.Description.TrimRight(EditorUtils.DescriptionMaxLength);
			AssetRowControl.TextBlockEditorPath.Text = File.Exists(Asset.EditorPath) ? Asset.EditorPath.TrimLeft(EditorUtils.EditorPathMaxLength) : GuiUtils.FileNotFound;
		}

		public void UpdateTagHighlighting(IEnumerable<string> checkedFilters, Color filterHighlightColor)
		{
			if (!checkedFilters.Any())
			{
				TextBlockTags.Text = string.Join(", ", Asset.Tags).TrimRight(EditorUtils.TagsMaxLength);
				return;
			}

			TextBlockTags.Inlines.Clear();

			int maxLength = EditorUtils.TagsMaxLength;
			int chars = 0;
			for (int i = 0; i < Asset.Tags.Length; i++)
			{
				string tag = Asset.Tags[i];
				chars += tag.Length;
				Run tagRun = new Run(chars > maxLength ? tag.TrimRight(tag.Length - (chars - maxLength)) : tag);
				if (checkedFilters.Contains(tag))
					tagRun.Background = new SolidColorBrush(filterHighlightColor);
				TextBlockTags.Inlines.Add(tagRun);
				if (i != Asset.Tags.Length - 1)
				{
					TextBlockTags.Inlines.Add(new Run(", "));
					chars += 2;
				}

				if (chars > maxLength)
					break;
			}
		}

		public virtual void BrowsePath()
		{
			OpenFileDialog openDialog = new OpenFileDialog { Filter = OpenDialogFilter };
			if (UserHandler.Instance.Settings.EnableAssetsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
				openDialog.InitialDirectory = UserHandler.Instance.Settings.AssetsRootFolder;

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Asset.EditorPath = openDialog.FileName.Replace("_fragment", string.Empty, StringComparison.InvariantCulture).Replace("_vertex", string.Empty, StringComparison.InvariantCulture);

			UpdateGui();
		}

		public void RemovePath()
		{
			Asset.EditorPath = GuiUtils.FileNotFound;

			UpdateGui();
		}
	}
}