using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
{
	public class DDFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.DD);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem modelBindingImport = new MenuItem { Header = "Import Model Binding paths from folder" };
			MenuItem modelImport = new MenuItem { Header = "Import Model paths from folder" };
			MenuItem shaderImport = new MenuItem { Header = "Import Shader paths from folder" };
			MenuItem textureImport = new MenuItem { Header = "Import Texture paths from folder" };

			modelBindingImport.Click += (sender, e) => App.Instance.MainWindow.DDModelBindingsAssetTabControl.Handler.ImportFolder();
			modelImport.Click += (sender, e) => App.Instance.MainWindow.DDModelsAssetTabControl.Handler.ImportFolder();
			shaderImport.Click += (sender, e) => App.Instance.MainWindow.DDShadersAssetTabControl.Handler.ImportFolder();
			textureImport.Click += (sender, e) => App.Instance.MainWindow.DDTexturesAssetTabControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(modelBindingImport);
			fileTypeMenuItem.Items.Add(modelImport);
			fileTypeMenuItem.Items.Add(shaderImport);
			fileTypeMenuItem.Items.Add(textureImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.DDModelBindingsAssetTabControl.Handler.Assets.Cast<AbstractAsset>()
				.Concat(App.Instance.MainWindow.DDModelsAssetTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDShadersAssetTabControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDTexturesAssetTabControl.Handler.Assets.Cast<AbstractAsset>())
				.ToList();
		}

		protected override void UpdateAssetTabControls(List<AbstractUserAsset> assets)
		{
			UpdateAssetTabControl(assets, App.Instance.MainWindow.DDModelBindingsAssetTabControl.Handler);
			UpdateAssetTabControl(assets, App.Instance.MainWindow.DDModelsAssetTabControl.Handler);
			UpdateAssetTabControl(assets, App.Instance.MainWindow.DDShadersAssetTabControl.Handler);
			UpdateAssetTabControl(assets, App.Instance.MainWindow.DDTexturesAssetTabControl.Handler);
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.DDModelBindingsAssetTabControl.Handler.IsComplete()
				&& App.Instance.MainWindow.DDModelsAssetTabControl.Handler.IsComplete()
				&& App.Instance.MainWindow.DDShadersAssetTabControl.Handler.IsComplete()
				&& App.Instance.MainWindow.DDTexturesAssetTabControl.Handler.IsComplete();
		}
	}
}