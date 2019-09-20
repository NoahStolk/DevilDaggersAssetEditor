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

			MenuItem coreShaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };

			coreShaderImport.Click += (sender, e) => App.Instance.MainWindow.CoreShadersExpanderControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(coreShaderImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.CoreShadersExpanderControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		protected override void UpdateExpanderControls(IEnumerable<GenericUserAsset> assets)
		{
			UpdateExpanderControl(assets, App.Instance.MainWindow.CoreShadersExpanderControl.Handler);
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.CoreShadersExpanderControl.Handler.IsComplete();
		}
	}
}