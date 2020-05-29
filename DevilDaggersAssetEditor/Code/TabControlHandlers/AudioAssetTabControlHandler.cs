using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class AudioAssetTabControlHandler : AbstractAssetTabControlHandler<AudioAsset, AudioAssetRowControl, AudioAssetRowControlHandler>
	{
		protected override string AssetTypeJsonFileName => "Audio";

		private UserSettings Settings => UserHandler.Instance.settings;

		public AudioAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public void ImportLoudness()
		{
			OpenFileDialog dialog = new OpenFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (Settings.EnableModsRootFolder)
				dialog.InitialDirectory = Settings.ModsRootFolder;
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
				if (!LoudnessUtils.ReadLoudnessLine(lineClean, out string assetName, out float loudness))
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
				AudioAssetRowControlHandler rowHandler = RowHandlers.FirstOrDefault(a => a.Asset.AssetName == kvp.Key);
				AudioAsset audioAsset = rowHandler.Asset;
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

					AudioAssetRowControl arc = rowHandler.AssetRowControl;
					arc.Handler.UpdateGui();
				}
			}

			App.Instance.ShowMessage("Loudness import results", $"Total audio assets: {RowHandlers.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}");
		}

		public void ExportLoudness()
		{
			SaveFileDialog dialog = new SaveFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (Settings.EnableModsRootFolder)
				dialog.InitialDirectory = Settings.ModsRootFolder;
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			StringBuilder sb = new StringBuilder();
			foreach (AudioAsset audioAsset in RowHandlers.Select(a => a.Asset))
				sb.AppendLine($"{audioAsset.AssetName} = {audioAsset.Loudness}");
			File.WriteAllText(dialog.FileName, sb.ToString());
		}
	}
}