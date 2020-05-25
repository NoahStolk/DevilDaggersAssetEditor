using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.PreviewerControls
{
	public partial class ModelPreviewerControl : UserControl
	{
		public ModelPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ModelAsset asset)
		{
			TextureName.Text = asset.AssetName;
			DefaultVertexCount.Text = asset.DefaultVertexCount.ToString();
			DefaultIndexCount.Text = asset.DefaultIndexCount.ToString();

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : Utils.FileNotFound;

			if (isPathValid)
			{
				string[] lines = File.ReadAllLines(asset.EditorPath);
				int v = 0;
				int vt = 0;
				int vn = 0;
				int f = 0;
				foreach (string line in lines)
				{
					switch (line.Split(' ')[0])
					{
						case "v": v++; break;
						case "vt": vt++; break;
						case "vn": vn++; break;
						case "f": f++; break;
					}
				}
				FileVertexCount.Text = new[] { v, vt, vn }.Max().ToString();
				FileIndexCount.Text = f.ToString();

				// TODO: Open in OBJ Viewer
			}
			else
			{
				FileVertexCount.Text = "N/A";
				FileIndexCount.Text = "N/A";
			}
		}
	}
}