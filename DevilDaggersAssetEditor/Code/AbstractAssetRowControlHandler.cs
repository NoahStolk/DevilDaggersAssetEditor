using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Info;
using DevilDaggersAssetCore.User;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
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

		protected readonly TAssetRowControl parent;
		protected readonly string openDialogFilter;

		public TextBlock TextBlockTags { get; }

		private Color colorInfoEven;
		private Color colorInfoOdd;
		private Color colorEven;
		private Color colorOdd;
		public Rectangle rectangleInfo = new Rectangle();
		public Rectangle rectangleEdit = new Rectangle();

		private UserSettings Settings => UserHandler.Instance.settings;

		protected AbstractAssetRowControlHandler(TAsset asset, TAssetRowControl parent, string openDialogFilter, TextBlock textBlockTags, bool isEven)
		{
			Asset = asset;
			this.parent = parent;
			this.openDialogFilter = openDialogFilter;
			TextBlockTags = textBlockTags;

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == Asset.GetType());
			colorEven = chunkInfo.GetColor() * 0.25f;
			colorOdd = colorEven * 0.5f;
			colorInfoEven = colorOdd;
			colorInfoOdd = colorOdd * 0.5f;

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
			rectangleInfo.Fill = new SolidColorBrush(isEven ? colorInfoEven : colorInfoOdd);
			rectangleEdit.Fill = new SolidColorBrush(isEven ? colorEven : colorOdd);
		}

		public abstract void UpdateGui();

		public void UpdateTagHighlighting(TextBlock textBlockTags, IEnumerable<string> checkedFilters, Color filterHighlightColor)
		{
			textBlockTags.Inlines.Clear();

			string[] assetTags = Asset.Tags;
			int maxLength = EditorUtils.TagsMaxLength;
			int chars = 0;
			for (int i = 0; i < assetTags.Length; i++)
			{
				string tag = assetTags[i];
				chars += tag.Length;
				Run tagRun = new Run(chars > maxLength ? tag.TrimRight(chars - maxLength) : tag);
				if (checkedFilters.Contains(tag))
					tagRun.Background = new SolidColorBrush(filterHighlightColor);
				TextBlockTags.Inlines.Add(tagRun);
				if (i != assetTags.Length - 1)
					TextBlockTags.Inlines.Add(new Run(", "));

				if (chars > maxLength)
					break;
			}
		}

		public virtual void BrowsePath()
		{
			OpenFileDialog openDialog = new OpenFileDialog { Filter = openDialogFilter };
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