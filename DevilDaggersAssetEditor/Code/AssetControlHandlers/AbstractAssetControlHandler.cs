using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.User;
using Microsoft.Win32;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public abstract class AbstractAssetControlHandler<TAsset, TAssetControl>
		where TAsset : AbstractAsset
		where TAssetControl : UserControl
	{
		public TAsset Asset { get; }
		protected readonly TAssetControl parent;
		protected readonly string openDialogFilter;

		public AbstractAssetControlHandler(TAsset asset, TAssetControl parent, string openDialogFilter)
		{
			Asset = asset;
			this.parent = parent;
			this.openDialogFilter = openDialogFilter;

			UpdateGui();
		}

		public abstract void UpdateGui();

		public virtual void BrowsePath()
		{
			OpenFileDialog openDialog = new OpenFileDialog { Filter = openDialogFilter, InitialDirectory = UserHandler.Instance.settings.AssetsRootFolder };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Asset.EditorPath = FileNameToChunkName(openDialog.FileName);

			UpdateGui();
		}

		public void RemovePath()
		{
			Asset.EditorPath = AbstractAsset.EditorPathNone;

			UpdateGui();
		}

		public virtual string FileNameToChunkName(string fileName) => fileName;
	}
}