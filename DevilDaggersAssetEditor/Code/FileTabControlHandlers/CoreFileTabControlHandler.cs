using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
{
	public class CoreFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Core);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem shaderImport = new MenuItem { Header = $"Import Shader paths from folder" };

			shaderImport.Click += (sender, e) => App.Instance.MainWindow.CoreShadersAssetTabControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(shaderImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets() => App.Instance.MainWindow.CoreShadersAssetTabControl.Handler.AssetRowEntries.Select(a => a.Asset).Cast<AbstractAsset>().ToList();

		public override void UpdateAssetTabControls(List<AbstractUserAsset> assets) => UpdateAssetTabControl(assets.OfType<ShaderUserAsset>().ToList(), App.Instance.MainWindow.CoreShadersAssetTabControl.Handler);

		protected override bool IsComplete() => App.Instance.MainWindow.CoreShadersAssetTabControl.Handler.IsComplete();
	}
}