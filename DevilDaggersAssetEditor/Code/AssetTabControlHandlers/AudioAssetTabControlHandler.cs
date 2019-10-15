using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.Previewers;
using DevilDaggersAssetEditor.Code.User;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.Code.AssetTabControlHandlers
{
	public class AudioAssetTabControlHandler : AbstractAssetTabControlHandler<AudioAsset, AudioAssetControl, AudioPreviewer>
	{
		protected override string AssetTypeJsonFileName => "Audio";

		public AudioAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGUI(AudioAsset asset)
		{
			AudioAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
			ac.TextBoxLoudness.Text = asset.Loudness.ToString();
		}

		public void ImportLoudness()
		{
			OpenFileDialog dialog = new OpenFileDialog { InitialDirectory = UserHandler.Instance.settings.ModsRootFolder, Filter = "Initialization files (*.ini)|*.ini" };
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Dictionary<string, float> values = new Dictionary<string, float>();
			int lineNumber = 0;
			foreach (string line in File.ReadAllLines(dialog.FileName))
			{
				lineNumber++;
				string lineClean = line
					.Replace(" ", "") // Remove spaces to make things easier.
					.TrimEnd('.'); // Remove dots at the end of the line. (The original loudness file has one on line 154 for some reason...)
				if (!ReadLoudnessLine(lineClean, out string assetName, out float loudness))
				{
					App.Instance.ShowMessage($"Syntax error on line {lineNumber}", "Could not parse loudness file.");
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

					AudioAssetControl aac = assetControls.Where(a => a.Handler.Asset == audioAsset).FirstOrDefault();
					aac.Handler.UpdateGUI();
				}
			}

			App.Instance.ShowMessage("Loudness import results", $"Total audio assets: {Assets.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}");

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

		public void ExportLoudness()
		{
			SaveFileDialog dialog = new SaveFileDialog { InitialDirectory = UserHandler.Instance.settings.ModsRootFolder, Filter = "Initialization files (*.ini)|*.ini" };
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			StringBuilder sb = new StringBuilder();
			foreach (AudioAsset audioAsset in Assets)
				sb.AppendLine($"{audioAsset.AssetName} = {audioAsset.Loudness}");
			File.WriteAllText(dialog.FileName, sb.ToString());
		}
	}
}