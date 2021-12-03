using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevilDaggersAssetEditor.Wpf.Utils;

public static class LoudnessWpfUtils
{
	public static void ImportLoudness(List<AssetRowControl> rowControls)
	{
		OpenFileDialog dialog = new() { Filter = "Initialization files (*.ini)|*.ini" };
		if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
			dialog.InitialDirectory = UserHandler.Instance.Settings.AssetsRootFolder;
		bool? openResult = dialog.ShowDialog();
		if (!openResult.HasValue || !openResult.Value)
			return;

		Dictionary<string, float> values = new();
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
			AssetRowControl? rowHandler = rowControls.Find(a => a.Asset.AssetName == kvp.Key);
			if (rowHandler != null)
			{
				AudioAsset audioAsset = (AudioAsset)rowHandler.Asset;
				if (audioAsset.Loudness == kvp.Value)
				{
					unchangedCount++;
				}
				else
				{
					audioAsset.Loudness = kvp.Value;
					successCount++;
				}

				rowHandler.UpdateGui();
			}
		}

		App.Instance.ShowMessage("Loudness import results", $"Total audio assets: {rowControls.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}");
	}

	public static void ExportLoudness(List<AssetRowControl> rowControls)
	{
		SaveFileDialog dialog = new() { Filter = "Initialization files (*.ini)|*.ini" };
		if (UserHandler.Instance.Settings.EnableModsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
			dialog.InitialDirectory = UserHandler.Instance.Settings.AssetsRootFolder;
		bool? result = dialog.ShowDialog();
		if (!result.HasValue || !result.Value)
			return;

		StringBuilder sb = new();
		foreach (AbstractAsset asset in rowControls.Select(a => a.Asset))
		{
			if (asset is AudioAsset audioAsset)
				sb.Append(audioAsset.AssetName).Append(" = ").Append(audioAsset.Loudness).AppendLine();
		}

		File.WriteAllText(dialog.FileName, sb.ToString());
	}
}