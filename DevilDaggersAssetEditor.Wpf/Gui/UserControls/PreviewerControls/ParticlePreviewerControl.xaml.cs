using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Utils;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
{
	public partial class ParticlePreviewerControl : UserControl, IPreviewerControl
	{
		public ParticlePreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(AbstractAsset a)
		{
			ParticleAsset asset = a as ParticleAsset;

			ParticleName.Content = asset.AssetName;

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Content = isPathValid ? Path.GetFileName(asset.EditorPath) : GuiUtils.FileNotFound;

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