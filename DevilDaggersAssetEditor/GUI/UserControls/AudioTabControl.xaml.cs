using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using DevilDaggersAssetCore.Assets;
using System;

namespace DevilDaggersAssetEditor.GUI.UserControls
{
	public partial class AudioTabControl : UserControl
	{
		private readonly BinaryFileName BinaryFileName = BinaryFileName.Audio;

		public List<AudioAsset> AudioAssets { get; private set; } = new List<AudioAsset>();

		private readonly List<AudioAssetControl> audioAssetControls = new List<AudioAssetControl>();

		public AudioTabControl()
		{
			InitializeComponent();

			using (StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream("DevilDaggersAssetCore.Content.Audio.Audio.json")))
				AudioAssets = JsonConvert.DeserializeObject<List<AudioAsset>>(sr.ReadToEnd());

			foreach (AudioAsset audioAsset in AudioAssets)
			{
				AudioAssetControl aac = new AudioAssetControl(audioAsset);
				audioAssetControls.Add(aac);
				AssetEditor.Children.Add(aac);
			}
		}

		private void Compress_Click(object sender, RoutedEventArgs e)
		{
			bool complete = true;
			foreach (AudioAsset audioAsset in AudioAssets)
			{
				if (!Utils.IsPathValid(audioAsset.EditorPath))
				{
					complete = false;
					break;
				}
			}

			if (!complete)
			{
				MessageBoxResult promptResult = MessageBox.Show("Not all file paths have been specified. In most cases this will cause Devil Daggers to crash on start up. Are you sure you wish to continue?", "Incomplete asset list", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (promptResult == MessageBoxResult.No)
					return;
			}

			SaveFileDialog dialog = new SaveFileDialog
			{
				InitialDirectory = Path.Combine(Utils.DDFolder, BinaryFileName.GetSubfolderName())
			};
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			Compressor.Compress(AudioAssets.Cast<AbstractAsset>().ToList(), dialog.FileName, BinaryFileName);
		}

		private void ImportFolder_Click(object sender, RoutedEventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
			{
				CommonFileDialogResult result = dialog.ShowDialog();
				if (result != CommonFileDialogResult.Ok)
					return;

				foreach (string filePath in Directory.GetFiles(dialog.FileName))
				{
					AudioAsset audioAsset = AudioAssets.Where(a => a.AssetName == Path.GetFileNameWithoutExtension(filePath)).Cast<AudioAsset>().FirstOrDefault();
					if (audioAsset != null)
					{
						audioAsset.EditorPath = filePath;

						// TODO: Fix binding
						AudioAssetControl aac = audioAssetControls.Where(a => a.AudioAsset == audioAsset).FirstOrDefault();
						aac.LabelEditorPath.Content = filePath;
					}
				}
			}
		}

		private void ImportLoudness_Click(object sender, RoutedEventArgs e)
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
				AudioAsset audioAsset = AudioAssets.Where(a => a.AssetName == kvp.Key).Cast<AudioAsset>().FirstOrDefault();
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
					AudioAssetControl aac = audioAssetControls.Where(a => a.AudioAsset == audioAsset).FirstOrDefault();
					aac.TextBoxLoudness.Text = audioAsset.Loudness.ToString();
				}
			}

			MessageBox.Show($"Total audio assets: {AudioAssets.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}", "Loudness import results");
		}

		private bool ReadLoudnessLine(string line, out string assetName, out float loudness)
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