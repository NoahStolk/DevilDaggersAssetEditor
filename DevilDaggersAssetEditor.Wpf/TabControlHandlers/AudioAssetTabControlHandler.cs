using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class AudioAssetTabControlHandler : AbstractAssetTabControlHandler<AudioAsset, AudioAssetRowControl, AudioAssetRowControlHandler>
	{
		public AudioAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		protected override string AssetTypeJsonFileName => "Audio";

		public void ImportLoudness()
		{
			OpenFileDialog dialog = new OpenFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
				dialog.InitialDirectory = UserHandler.Instance.Settings.AssetsRootFolder;
			bool? openResult = dialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			Dictionary<string, float> values = new Dictionary<string, float>();
			int lineNumber = 0;
			foreach (string line in File.ReadAllLines(dialog.FileName))
			{
				lineNumber++;
				if (!LoudnessUtils.TryReadLoudnessLine(line, out string? assetName, out float loudness))
				{
					App.Instance.ShowMessage($"Syntax error on line {lineNumber}", "Could not parse loudness file.");
					return;
				}

				values[assetName!] = loudness;
			}

			int successCount = 0;
			int unchangedCount = 0;
			foreach (KeyValuePair<string, float> kvp in values)
			{
				AudioAssetRowControlHandler rowHandler = RowHandlers.FirstOrDefault(a => a.Asset.AssetName == kvp.Key);
				if (rowHandler != null)
				{
					AudioAsset audioAsset = rowHandler.Asset;
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
			if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
				dialog.InitialDirectory = UserHandler.Instance.Settings.AssetsRootFolder;
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