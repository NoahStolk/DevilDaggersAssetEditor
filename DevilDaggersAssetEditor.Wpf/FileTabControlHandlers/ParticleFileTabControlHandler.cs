using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.FileTabControlHandlers
{
	public class ParticleFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ParticleFileHandler();

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem particleBindingImport = new MenuItem { Header = $"Import Particle paths from folder" };

			particleBindingImport.Click += (sender, e) => App.Instance.MainWindow!.ParticleParticlesAssetTabControl.ImportFolder();

			fileTypeMenuItem.Items.Add(particleBindingImport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
			=> App.Instance.MainWindow!.ParticleParticlesAssetTabControl.RowHandlers.Select(a => a.Asset).ToList();

		public override void UpdateAssetTabControls(List<UserAsset> assets)
			=> UpdateAssetTabControl(assets.Where(a => a.AssetType == AssetType.Particle).ToList(), App.Instance.MainWindow!.ParticleParticlesAssetTabControl);

		protected override bool IsComplete()
			=> App.Instance.MainWindow!.ParticleParticlesAssetTabControl.IsComplete();
	}
}