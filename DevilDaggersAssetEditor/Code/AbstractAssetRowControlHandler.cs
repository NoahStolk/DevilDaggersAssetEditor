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
		private readonly Color colorInfoEven;
		private readonly Color colorInfoOdd;
		private readonly Color colorEven;
		private readonly Color colorOdd;
		public readonly Rectangle rectangleInfo;
		public readonly Rectangle rectangle;

		private UserSettings settings => UserHandler.Instance.settings;

		public AbstractAssetRowControlHandler(TAsset asset, TAssetRowControl parent, string openDialogFilter, bool isEven)
		{
			Asset = asset;
			this.parent = parent;
			this.openDialogFilter = openDialogFilter;

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == asset.GetType());
			colorInfoEven = chunkInfo.GetColor() * 0.25f;
			colorInfoOdd = chunkInfo.GetColor() * 0.125f;
			colorEven = colorInfoEven * 0.5f;
			colorOdd = colorInfoOdd * 0.5f;

			rectangleInfo = new Rectangle { Fill = new SolidColorBrush(isEven ? colorInfoEven : colorInfoOdd) };
			Panel.SetZIndex(rectangleInfo, -1);
			Grid.SetColumnSpan(rectangleInfo, 3);
			rectangle = new Rectangle { Fill = new SolidColorBrush(isEven ? colorEven : colorOdd) };
			Panel.SetZIndex(rectangle, -1);
			Grid.SetColumn(rectangle, 3);
			Grid.SetColumnSpan(rectangle, 4);

			UpdateGui();
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