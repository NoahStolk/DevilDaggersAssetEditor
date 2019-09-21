using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class DDTabControlHandler : AbstractTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.DD);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem modelBindingImport = new MenuItem { Header = $"Import Model Binding paths from folder", IsEnabled = false };
			MenuItem modelImport = new MenuItem { Header = $"Import Model paths from folder", IsEnabled = false };
			MenuItem shaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };
			MenuItem textureImport = new MenuItem { Header = $"Import Texture paths from folder", IsEnabled = false };

			modelBindingImport.Click += (sender, e) => App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.ImportFolder();
			modelImport.Click += (sender, e) => App.Instance.MainWindow.DDModelsExpanderControl.Handler.ImportFolder();
			shaderImport.Click += (sender, e) => App.Instance.MainWindow.DDShadersExpanderControl.Handler.ImportFolder();
			textureImport.Click += (sender, e) => App.Instance.MainWindow.DDTexturesExpanderControl.Handler.ImportFolder();
			
			fileTypeMenuItem.Items.Add(modelBindingImport);
			fileTypeMenuItem.Items.Add(modelImport);
			fileTypeMenuItem.Items.Add(shaderImport);
			fileTypeMenuItem.Items.Add(textureImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.Assets.Cast<AbstractAsset>()
				.Concat(App.Instance.MainWindow.DDModelsExpanderControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDShadersExpanderControl.Handler.Assets.Cast<AbstractAsset>())
				.Concat(App.Instance.MainWindow.DDTexturesExpanderControl.Handler.Assets.Cast<AbstractAsset>())
				.ToList();
		}

		protected override void UpdateExpanderControls(List<GenericUserAsset> assets)
		{
			UpdateExpanderControl(assets, App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler);
			UpdateExpanderControl(assets, App.Instance.MainWindow.DDModelsExpanderControl.Handler);
			UpdateExpanderControl(assets, App.Instance.MainWindow.DDShadersExpanderControl.Handler);
			UpdateExpanderControl(assets, App.Instance.MainWindow.DDTexturesExpanderControl.Handler);
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.IsComplete()
				&& App.Instance.MainWindow.DDModelsExpanderControl.Handler.IsComplete()
				&& App.Instance.MainWindow.DDShadersExpanderControl.Handler.IsComplete()
				&& App.Instance.MainWindow.DDTexturesExpanderControl.Handler.IsComplete();
		}
	}
}