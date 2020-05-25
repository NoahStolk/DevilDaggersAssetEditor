using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Info;
using DevilDaggersAssetCore.User;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Controls;
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

		private Color colorInfoEven;
		private Color colorInfoOdd;
		private Color colorEven;
		private Color colorOdd;
		public Rectangle rectangleInfo = new Rectangle();
		public Rectangle rectangleEdit = new Rectangle();

		private UserSettings settings => UserHandler.Instance.settings;

		public AbstractAssetRowControlHandler(TAsset asset, TAssetRowControl parent, string openDialogFilter, bool isEven)
		{
			Asset = asset;
			this.parent = parent;
			this.openDialogFilter = openDialogFilter;

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

		public virtual void BrowsePath()
		{
			OpenFileDialog openDialog = new OpenFileDialog { Filter = openDialogFilter };
			if (settings.EnableAssetsRootFolder)
				openDialog.InitialDirectory = settings.AssetsRootFolder;

			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Asset.EditorPath = FileNameToChunkName(openDialog.FileName);

			UpdateGui();
		}

		public void RemovePath()
		{
			Asset.EditorPath = Utils.FileNotFound;

			UpdateGui();
		}

		public virtual string FileNameToChunkName(string fileName) => fileName;
	}
}