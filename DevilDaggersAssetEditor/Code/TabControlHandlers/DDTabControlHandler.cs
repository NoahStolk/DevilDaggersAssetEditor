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

			MenuItem ddModelBindingImport = new MenuItem { Header = $"Import Model Binding paths from folder", IsEnabled = false };
			MenuItem ddModelImport = new MenuItem { Header = $"Import Model paths from folder", IsEnabled = false };
			MenuItem ddShaderImport = new MenuItem { Header = $"Import Shader paths from folder", IsEnabled = false };
			MenuItem ddTextureImport = new MenuItem { Header = $"Import Texture paths from folder", IsEnabled = false };

			ddModelBindingImport.Click += (sender, e) => App.Instance.MainWindow.DDModelBindingsExpanderControl.Handler.ImportFolder();
			ddModelImport.Click += (sender, e) => App.Instance.MainWindow.DDModelsExpanderControl.Handler.ImportFolder();
			ddShaderImport.Click += (sender, e) => App.Instance.MainWindow.DDShadersExpanderControl.Handler.ImportFolder();
			ddTextureImport.Click += (sender, e) => App.Instance.MainWindow.DDTexturesExpanderControl.Handler.ImportFolder();
			
			fileTypeMenuItem.Items.Add(ddModelBindingImport);
			fileTypeMenuItem.Items.Add(ddModelImport);
			fileTypeMenuItem.Items.Add(ddShaderImport);
			fileTypeMenuItem.Items.Add(ddTextureImport);

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

		protected override void UpdateExpanderControls(IEnumerable<GenericUserAsset> assets)
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