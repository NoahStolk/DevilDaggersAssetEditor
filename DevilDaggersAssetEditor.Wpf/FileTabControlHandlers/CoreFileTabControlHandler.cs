using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.FileTabControlHandlers
{
	public class CoreFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Core);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem shaderImport = new MenuItem { Header = $"Import Shader paths from folder" };

			shaderImport.Click += (sender, e) => App.Instance.MainWindow!.CoreShadersAssetTabControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(shaderImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets() => App.Instance.MainWindow!.CoreShadersAssetTabControl.Handler.RowHandlers.Select(a => a.Asset).Cast<AbstractAsset>().ToList();

		public override void UpdateAssetTabControls(List<AbstractUserAsset> assets) => UpdateAssetTabControl(assets.OfType<ShaderUserAsset>().ToList(), App.Instance.MainWindow!.CoreShadersAssetTabControl.Handler);

		protected override bool IsComplete() => App.Instance.MainWindow!.CoreShadersAssetTabControl.Handler.IsComplete();
	}
}