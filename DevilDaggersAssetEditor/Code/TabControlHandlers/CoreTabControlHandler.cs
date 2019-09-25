using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class CoreTabControlHandler : AbstractTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Core);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem shaderImport = new MenuItem { Header = $"Import Shader paths from folder" };

			shaderImport.Click += (sender, e) => App.Instance.MainWindow.CoreShadersExpanderControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(shaderImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.CoreShadersExpanderControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		protected override void UpdateExpanderControls(List<AbstractUserAsset> assets)
		{
			UpdateExpanderControl(assets, App.Instance.MainWindow.CoreShadersExpanderControl.Handler);
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.CoreShadersExpanderControl.Handler.IsComplete();
		}
	}
}