using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class AudioTabControlHandler : AbstractTabControlHandler<AudioAsset, AudioAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Audio";

		public AudioTabControlHandler(BinaryFileName binaryFileName)
			: base(binaryFileName)
		{
		}

		protected override void UpdatePathLabel(AudioAsset asset)
		{
			AudioAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}

		public void ImportLoudness()
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Dictionary<string, float> values = new Dictionary<string, float>();
			int lineNumber = 0;
			foreach (string line in File.ReadAllLines(openDialog.FileName))
			{
				lineNumber++;
				string lineClean = line
					.Replace(" ", "") // Remove spaces to make things easier.
					.TrimEnd('.'); // Remove dots at the end of the line. (The original loudness file has one on line 154 for some reason...)
				if (!ReadLoudnessLine(lineClean, out string assetName, out float loudness))
				{
					MessageBox.Show($"Syntax error on line {lineNumber}", "Could not parse loudness file");
					return;
				}

				values[assetName] = loudness;
			}

			int successCount = 0;
			int unchangedCount = 0;
			foreach (KeyValuePair<string, float> kvp in values)
			{
				AudioAsset audioAsset = Assets.Where(a => a.AssetName == kvp.Key).Cast<AudioAsset>().FirstOrDefault();
				if (audioAsset != null)
				{
					if (audioAsset.Loudness == kvp.Value)
					{
						unchangedCount++;
					}
					else
					{
						audioAsset.Loudness = kvp.Value;
						successCount++;
					}

					// TODO: Fix binding
					AudioAssetControl aac = assetControls.Where(a => a.Handler.Asset == audioAsset).FirstOrDefault();
					aac.TextBoxLoudness.Text = audioAsset.Loudness.ToString();
				}
			}

			MessageBox.Show($"Total audio assets: {Assets.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}", "Loudness import results");

			bool ReadLoudnessLine(string line, out string assetName, out float loudness)
			{
				try
				{
					assetName = line.Substring(0, line.IndexOf('='));
					loudness = float.Parse(line.Substring(line.IndexOf('=') + 1, line.Length - assetName.Length - 1));
					return true;
				}
				catch
				{
					assetName = null;
					loudness = 0;
					return false;
				}
			}
		}
	}
}