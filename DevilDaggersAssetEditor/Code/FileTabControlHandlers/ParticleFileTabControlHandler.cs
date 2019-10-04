﻿using DevilDaggersAssetCore.Assets;
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

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		protected override void UpdateAssetTabControls(List<AbstractUserAsset> assets)
		{
			UpdateAssetTabControl(assets, App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler);
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.ParticleParticlesAssetTabControl.Handler.IsComplete();
		}
	}
}