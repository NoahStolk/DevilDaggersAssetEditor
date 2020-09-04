using DevilDaggersAssetEditor.Assets;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
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

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : Utils.FileNotFound;

			if (isPathValid)
			{
				byte[] bytes = File.ReadAllBytes(asset.EditorPath);
				string hex = BitConverter.ToString(bytes).Replace("-", string.Empty, StringComparison.InvariantCulture);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hex.Length; i++)
				{
					if (i != 0)
					{
						if (i % 96 == 0)
							sb.Append('\n');
						else if (i % 8 == 0)
							sb.Append(' ');
					}

					sb.Append(hex[i]);
				}

				PreviewTextBox.Text = sb.ToString();
			}
			else
			{
				PreviewTextBox.Text = string.Empty;
			}
		}
	}
}