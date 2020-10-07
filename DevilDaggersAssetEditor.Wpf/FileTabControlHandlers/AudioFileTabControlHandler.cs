using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.FileTabControlHandlers
{
	public class AudioFileTabControlHandler : AbstractFileTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Audio);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem audioImport = new MenuItem { Header = $"Import Audio paths from folder" };
			MenuItem loudnessImport = new MenuItem { Header = $"Import Loudness from file" };
			MenuItem loudnessExport = new MenuItem { Header = $"Export Loudness to file" };

			audioImport.Click += (sender, e) => App.Instance.MainWindow!.AudioAudioAssetTabControl.Handler.ImportFolder();
			loudnessImport.Click += (sender, e) => App.Instance.MainWindow!.AudioAudioAssetTabControl.Handler.ImportLoudness();
			loudnessExport.Click += (sender, e) => App.Instance.MainWindow!.AudioAudioAssetTabControl.Handler.ExportLoudness();

			fileTypeMenuItem.Items.Add(audioImport);
			fileTypeMenuItem.Items.Add(new Separator());
			fileTypeMenuItem.Items.Add(loudnessImport);
			fileTypeMenuItem.Items.Add(loudnessExport);

			return fileTypeMenuItem;
		}

		public override List<AbstractAsset> GetAssets()
			=> App.Instance.MainWindow!.AudioAudioAssetTabControl.Handler.RowHandlers.Select(a => a.Asset).Cast<AbstractAsset>().ToList();

		public override void UpdateAssetTabControls(List<AbstractUserAsset> assets)
			=> UpdateAssetTabControl(assets.OfType<AudioUserAsset>().ToList(), App.Instance.MainWindow!.AudioAudioAssetTabControl.Handler);

		protected override bool IsComplete()
			=> App.Instance.MainWindow!.AudioAudioAssetTabControl.Handler.IsComplete();
	}
}