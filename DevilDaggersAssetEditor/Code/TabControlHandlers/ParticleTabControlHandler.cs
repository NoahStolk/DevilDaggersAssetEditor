using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class ParticleTabControlHandler : AbstractTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ParticleFileHandler();

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem particleBindingImport = new MenuItem { Header = $"Import Particle paths from folder" };

			particleBindingImport.Click += (sender, e) => App.Instance.MainWindow.ParticleParticlesExpanderControl.Handler.ImportFolder();

			fileTypeMenuItem.Items.Add(particleBindingImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.ParticleParticlesExpanderControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		protected override void UpdateExpanderControls(List<AbstractUserAsset> assets)
		{
			UpdateExpanderControl(assets, App.Instance.MainWindow.ParticleParticlesExpanderControl.Handler);
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.ParticleParticlesExpanderControl.Handler.IsComplete();
		}
	}
}