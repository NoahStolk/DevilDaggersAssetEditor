using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.User;
using Microsoft.Win32;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	internal abstract class AbstractAssetRowControlHandler<TAsset, TAssetControl>
		where TAsset : AbstractAsset
		where TAssetControl : UserControl
	{
		internal TAsset Asset { get; }
		protected readonly TAssetControl parent;
		protected readonly string openDialogFilter;

		internal AbstractAssetRowControlHandler(TAsset asset, TAssetControl parent, string openDialogFilter)
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