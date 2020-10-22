using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.FileTabControlHandlers
{
	public class DdFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Dd);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem modelBindingImport = new MenuItem { Header = "Import Model Binding paths from folder" };
			MenuItem modelImport = new MenuItem { Header = "Import Model paths from folder" };
			MenuItem shaderImport = new MenuItem { Header = "Import Shader paths from folder" };
			MenuItem textureImport = new MenuItem { Header = "Import Texture paths from folder" };

			modelBindingImport.Click += (sender, e) => App.Instance.MainWindow!.DdModelBindingsAssetTabControl.ImportFolder();
			modelImport.Click += (sender, e) => App.Instance.MainWindow!.DdModelsAssetTabControl.ImportFolder();
			shaderImport.Click += (sender, e) => App.Instance.MainWindow!.DdShadersAssetTabControl.ImportFolder();
			textureImport.Click += (sender, e) => App.Instance.MainWindow!.DdTexturesAssetTabControl.ImportFolder();

			fileTypeMenuItem.Items.Add(modelBindingImport);
			fileTypeMenuItem.Items.Add(modelImport);
			fileTypeMenuItem.Items.Add(shaderImport);
			fileTypeMenuItem.Items.Add(textureImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
			=> App.Instance.MainWindow!.DdModelBindingsAssetTabControl.RowHandlers.Select(a => a.Asset)
				.Concat(App.Instance.MainWindow!.DdModelsAssetTabControl.RowHandlers.Select(a => a.Asset))
				.Concat(App.Instance.MainWindow!.DdShadersAssetTabControl.RowHandlers.Select(a => a.Asset))
				.Concat(App.Instance.MainWindow!.DdTexturesAssetTabControl.RowHandlers.Select(a => a.Asset))
				.ToList();

		public override void UpdateAssetTabControls(List<UserAsset> assets)
		{
			UpdateAssetTabControl(assets.Where(a => a.AssetType == AssetType.ModelBinding).ToList(), App.Instance.MainWindow!.DdModelBindingsAssetTabControl);
			UpdateAssetTabControl(assets.Where(a => a.AssetType == AssetType.Model).ToList(), App.Instance.MainWindow!.DdModelsAssetTabControl);
			UpdateAssetTabControl(assets.Where(a => a.AssetType == AssetType.Shader).ToList(), App.Instance.MainWindow!.DdShadersAssetTabControl);
			UpdateAssetTabControl(assets.Where(a => a.AssetType == AssetType.Texture).ToList(), App.Instance.MainWindow!.DdTexturesAssetTabControl);
		}

		protected override bool IsComplete()
			=> App.Instance.MainWindow!.DdModelBindingsAssetTabControl.IsComplete()
			&& App.Instance.MainWindow!.DdModelsAssetTabControl.IsComplete()
			&& App.Instance.MainWindow!.DdShadersAssetTabControl.IsComplete()
			&& App.Instance.MainWindow!.DdTexturesAssetTabControl.IsComplete();
	}
}