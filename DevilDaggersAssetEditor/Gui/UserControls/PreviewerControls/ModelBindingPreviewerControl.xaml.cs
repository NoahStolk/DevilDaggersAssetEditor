﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ModelBindingPreviewerControl : UserControl
	{
		public ModelBindingPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ModelBindingAsset asset)
		{
			ModelBindingName.Text = asset.AssetName;

			bool isPathValid = asset.EditorPath.GetPathValidity() == PathValidity.Valid;

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : Utils.GetPathValidityMessage(asset.EditorPath);

			if (isPathValid)
				PreviewTextBox.Text = File.ReadAllText(asset.EditorPath);
			else
				PreviewTextBox.Text = string.Empty;
		}
	}
}