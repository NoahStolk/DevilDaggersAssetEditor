using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
{
	public class ParticleFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ParticleFileHandler();

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem particleBindingImport = new MenuItem { Header = $"Import Particle paths from folder" };

			particleBindingImport.Click += (sender, e) => App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(particleBindingImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets() => App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.RowHandlers.Select(a => a.Asset).Cast<AbstractAsset>().ToList();

		public override void UpdateAssetTabControls(List<AbstractUserAsset> assets) => UpdateAssetTabControl(assets.OfType<ParticleUserAsset>().ToList(), App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler);

		protected override bool IsComplete() => App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.IsComplete();
	}
}