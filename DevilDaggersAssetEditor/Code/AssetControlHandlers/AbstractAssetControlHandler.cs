using DevilDaggersAssetCore.Assets;
using Microsoft.Win32;

namespace DevilDaggersAssetEditor.Code.AssetControlHandlers
{
	public abstract class AbstractAssetControlHandler<TAsset, TAssetControl> where TAsset : AbstractAsset
	{
		public TAsset Asset { get; }
		protected readonly TAssetControl parent;
		private readonly string openDialogFilter;

		public AbstractAssetControlHandler(TAsset asset, TAssetControl parent, string openDialogFilter)
		{
			Asset = asset;
			this.parent = parent;
			this.openDialogFilter = openDialogFilter;

			UpdateGUI();
		}

		public abstract void UpdateGUI();

		public void BrowsePath()
		{
			OpenFileDialog openDialog = new OpenFileDialog { Filter = openDialogFilter };
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Asset.EditorPath = openDialog.FileName;

			UpdateGUI();
		}
	}
}