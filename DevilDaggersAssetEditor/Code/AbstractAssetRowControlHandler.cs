using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Info;
using DevilDaggersAssetCore.User;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersAssetEditor.Code
{
	public abstract class AbstractAssetRowControlHandler<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		public TAsset Asset { get; }
		public TAssetRowControl AssetRowControl { get; }

		public abstract string OpenDialogFilter { get; }

		public TextBlock TextBlockTags { get; }

		private readonly Color colorInfoEven;
		private readonly Color colorInfoOdd;
		private readonly Color colorEditEven;
		private readonly Color colorEditOdd;
		private readonly SolidColorBrush brushInfoEven;
		private readonly SolidColorBrush brushInfoOdd;
		private readonly SolidColorBrush brushEditEven;
		private readonly SolidColorBrush brushEditOdd;

		public Rectangle rectangleInfo = new Rectangle();
		public Rectangle rectangleEdit = new Rectangle();

		private UserSettings Settings => UserHandler.Instance.settings;

		protected AbstractAssetRowControlHandler(TAsset asset, bool isEven)
		{
			Asset = asset;
			TextBlockTags = new TextBlock
			{
				Text = string.Join(", ", asset.Tags).TrimRight(EditorUtils.TagsMaxLength),
				Margin = new Thickness(2)
			};
			Grid.SetColumn(TextBlockTags, 1);

			AssetRowControl = (TAssetRowControl)Activator.CreateInstance(typeof(TAssetRowControl), this);

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == Asset.GetType());
			colorEditEven = chunkInfo.GetColor() * 0.25f;
			colorEditOdd = colorEditEven * 0.5f;
			colorInfoEven = colorEditOdd;
			colorInfoOdd = colorEditOdd * 0.5f;
			brushInfoEven = new SolidColorBrush(colorInfoEven);
			brushInfoOdd = new SolidColorBrush(colorInfoOdd);
			brushEditEven = new SolidColorBrush(colorEditEven);
			brushEditOdd = new SolidColorBrush(colorEditOdd);

			UpdateGui();

			Panel.SetZIndex(rectangleInfo, -1);
			Grid.SetColumnSpan(rectangleInfo, 3);

			Panel.SetZIndex(rectangleEdit, -1);
			Grid.SetColumn(rectangleEdit, 3);
			Grid.SetColumnSpan(rectangleEdit, 4);

			UpdateBackgroundRectangleColors(isEven);
		}

		public void UpdateBackgroundRectangleColors(bool isEven)
		{
			rectangleInfo.Fill = isEven ? brushInfoEven : brushInfoOdd;
			rectangleEdit.Fill = isEven ? brushEditEven : brushEditOdd;
		}

		public abstract void UpdateGui();

		public void UpdateTagHighlighting(IEnumerable<string> checkedFilters, Color filterHighlightColor)
		{
			if (checkedFilters.Count() == 0)
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
			if (Settings.EnableAssetsRootFolder)
				openDialog.InitialDirectory = Settings.AssetsRootFolder;

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Asset.EditorPath = openDialog.FileName.Replace("_fragment", "").Replace("_vertex", "");

			UpdateGui();
		}

		public void RemovePath()
		{
			Asset.EditorPath = Utils.FileNotFound;

			UpdateGui();
		}
	}
}