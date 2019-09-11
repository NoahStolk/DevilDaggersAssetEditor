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


		}
	}
}