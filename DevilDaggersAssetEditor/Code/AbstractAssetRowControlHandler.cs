using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.User;
using Microsoft.Win32;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	internal abstract class AbstractAssetRowControlHandler<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		internal TAsset Asset { get; }
		private protected readonly TAssetRowControl parent;
		private protected readonly string openDialogFilter;

		internal AbstractAssetRowControlHandler(TAsset asset, TAssetRowControl parent, string openDialogFilter)
		{
			Asset = asset;
			this.parent = parent;
			this.openDialogFilter = openDialogFilter;

			UpdateGui();
		}

		internal abstract void UpdateGui();

		internal virtual void BrowsePath()
		{
			OpenFileDialog openDialog = new OpenFileDialog { Filter = openDialogFilter, InitialDirectory = UserHandler.Instance.settings.AssetsRootFolder };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Asset.EditorPath = FileNameToChunkName(openDialog.FileName);

			UpdateGui();
		}

		internal void RemovePath()
		{
			Asset.EditorPath = AbstractAsset.EditorPathNone;

			UpdateGui();
		}

		internal virtual string FileNameToChunkName(string fileName) => fileName;
	}
}