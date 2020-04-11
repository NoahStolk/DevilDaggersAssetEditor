using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
{
	internal class ParticleFileTabControlHandler : AbstractFileTabControlHandler
	{
		internal override AbstractBinaryFileHandler FileHandler => new ParticleFileHandler();

		internal override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem particleBindingImport = new MenuItem { Header = $"Import Particle paths from folder" };

			particleBindingImport.Click += (sender, e) => App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(particleBindingImport);

			return fileTypeMenuItem;
		}

		internal override List<AbstractAsset> GetAssets() => App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();

		private protected override void UpdateAssetTabControls(List<AbstractUserAsset> assets) => UpdateAssetTabControl(assets, App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler);

		private protected override bool IsComplete() => App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.IsComplete();
	}
}