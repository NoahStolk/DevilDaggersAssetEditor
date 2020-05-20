﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class ParticleAssetRowControl : UserControl
	{
		public ParticleAssetRowControlHandler Handler { get; private set; }

		public ParticleAssetRowControl(ParticleAsset asset)
		{
			InitializeComponent();

			Handler = new ParticleAssetRowControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();
	}

	public class ParticleAssetRowControlHandler : AbstractAssetRowControlHandler<ParticleAsset, ParticleAssetRowControl>
	{
		public ParticleAssetRowControlHandler(ParticleAsset asset, ParticleAssetRowControl parent)
			: base(asset, parent, "Particle files (*.bin)|*.bin")
		{
		}

		public override void UpdateGui()
		{
			bool isPathValid = Asset.EditorPath.GetPathValidity() == PathValidity.Valid;
			parent.TextBlockEditorPath.Text = isPathValid ? Asset.EditorPath : Utils.GetPathValidityMessage(Asset.EditorPath);
		}
	}
}