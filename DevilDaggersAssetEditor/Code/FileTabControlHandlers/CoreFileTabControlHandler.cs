using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
{
	internal class CoreFileTabControlHandler : AbstractFileTabControlHandler
	{
		internal override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Core);

		internal override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem shaderImport = new MenuItem { Header = $"Import Shader paths from folder" };

			shaderImport.Click += (sender, e) => App.Instance.MainWindow.CoreShadersAssetTabControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(shaderImport);

			return fileTypeMenuItem;
		}

		internal override List<AbstractAsset> GetAssets() => App.Instance.MainWindow.CoreShadersAssetTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();

		private protected override void UpdateAssetTabControls(List<AbstractUserAsset> assets) => UpdateAssetTabControl(assets, App.Instance.MainWindow.CoreShadersAssetTabControl.Handler);

		private protected override bool IsComplete() => App.Instance.MainWindow.CoreShadersAssetTabControl.Handler.IsComplete();
	}
}