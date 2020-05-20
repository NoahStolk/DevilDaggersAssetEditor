using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ParticlePreviewerControl : UserControl
	{
		public ParticlePreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ParticleAsset asset)
		{
			ParticleName.Text = asset.AssetName;

			bool isPathValid = asset.EditorPath.GetPathValidity() == PathValidity.Valid;

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : Utils.GetPathValidityMessage(asset.EditorPath);

			if (isPathValid)
			{
				byte[] bytes = File.ReadAllBytes(asset.EditorPath);
				PreviewTextBox.Text = Regex.Replace(BitConverter.ToString(bytes).Replace("-", string.Empty), $".{{{bytes.Length / 2}}}", "$0\n").TrimEnd('\n');
			}
			else
			{
				PreviewTextBox.Text = string.Empty;
			}
		}
	}
}