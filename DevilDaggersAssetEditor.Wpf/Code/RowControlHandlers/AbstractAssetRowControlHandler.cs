using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Info;
using DevilDaggersAssetEditor.User;
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

namespace DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers
{
	public abstract class AbstractAssetRowControlHandler<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		private readonly Color _colorInfoEven;
		private readonly Color _colorInfoOdd;
		private readonly Color _colorEditEven;
		private readonly Color _colorEditOdd;
		private readonly SolidColorBrush _brushInfoEven;
		private readonly SolidColorBrush _brushInfoOdd;
		private readonly SolidColorBrush _brushEditEven;
		private readonly SolidColorBrush _brushEditOdd;

		protected AbstractAssetRowControlHandler(TAsset asset, bool isEven)
		{
			Asset = asset;
			TextBlockTags = new TextBlock
			{
				Text = string.Join(", ", asset.Tags).TrimRight(EditorUtils.TagsMaxLength),
				Margin = new Thickness(2),
			};
			Grid.SetColumn(TextBlockTags, 1);

			AssetRowControl = (TAssetRowControl)Activator.CreateInstance(typeof(TAssetRowControl), this);

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == Asset.GetType());
			_colorEditEven = chunkInfo.GetColor() * 0.25f;
			_colorEditOdd = _colorEditEven * 0.5f;
			_colorInfoEven = _colorEditOdd;
			_colorInfoOdd = _colorEditOdd * 0.5f;
			_brushInfoEven = new SolidColorBrush(_colorInfoEven);
			_brushInfoOdd = new SolidColorBrush(_colorInfoOdd);
			_brushEditEven = new SolidColorBrush(_colorEditEven);
			_brushEditOdd = new SolidColorBrush(_colorEditOdd);

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

		public TAsset Asset { get; }
		public TAssetRowControl AssetRowControl { get; }

		public bool IsActive { get; set; } = true;

		public abstract string OpenDialogFilter { get; }

		public TextBlock TextBlockTags { get; }

		public void UpdateBackgroundRectangleColors(bool isEven)
		{
			RectangleInfo.Fill = isEven ? _brushInfoEven : _brushInfoOdd;
			RectangleEdit.Fill = isEven ? _brushEditEven : _brushEditOdd;
		}

		public abstract void UpdateGui();

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
			Asset.EditorPath = Utils.FileNotFound;

			UpdateGui();
		}
	}
}