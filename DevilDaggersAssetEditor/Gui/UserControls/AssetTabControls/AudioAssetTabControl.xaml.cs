using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.User;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetTabControls
{
	public partial class AudioAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(AudioAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AudioAssetTabControlHandler Handler { get; private set; }

		public AudioAssetTabControl()
		{
			InitializeComponent();

			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
			timer.Tick += (sender, e) =>
			{
				if (Previewer.Song == null || Previewer.Song.Paused)
					return;

				if (!Previewer.Dragging)
					Previewer.Seek.Value = Previewer.Song.PlayPosition / (float)Previewer.Song.PlayLength * Previewer.Seek.Maximum;

				Previewer.SeekText.Text = $"{EditorUtils.ToTimeString((int)Previewer.Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Previewer.Song.PlayLength)}";
			};
			timer.Start();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (AudioAssetRowControl arc in Handler.CreateAssetRowControls())
				AssetEditor.Items.Add(arc);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AudioAssetRowControl arc = e.AddedItems[0] as AudioAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}
	}

	public class AudioAssetTabControlHandler : AbstractAssetTabControlHandler<AudioAsset, AudioAssetRowControl>
	{
		protected override string AssetTypeJsonFileName => "Audio";

		private UserSettings settings => UserHandler.Instance.settings;

		public AudioAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(AudioAsset asset)
		{
			AudioAssetRowControl arc = assetRowControls.FirstOrDefault(a => a.Handler.Asset == asset);
			arc.TextBlockEditorPath.Text = asset.EditorPath;
			arc.TextBoxLoudness.Text = asset.Loudness.ToString();
			arc.Handler.UpdateGui();
		}

		public void ImportLoudness()
		{
			OpenFileDialog dialog = new OpenFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (settings.EnableModsRootFolder)
				dialog.InitialDirectory = settings.ModsRootFolder;
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

					AudioAssetRowControl aac = assetRowControls.FirstOrDefault(a => a.Handler.Asset == audioAsset);
					aac.Handler.UpdateGui();
				}
			}

			App.Instance.ShowMessage("Loudness import results", $"Total audio assets: {Assets.Count}\nAudio assets found in specified loudness file: {values.Count}\n\nUpdated: {successCount} / {values.Count}\nUnchanged: {unchangedCount} / {values.Count}\nNot found: {values.Count - (successCount + unchangedCount)} / {values.Count}");
		}

		public void ExportLoudness()
		{
			SaveFileDialog dialog = new SaveFileDialog { Filter = "Initialization files (*.ini)|*.ini" };
			if (settings.EnableModsRootFolder)
				dialog.InitialDirectory = settings.ModsRootFolder;
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