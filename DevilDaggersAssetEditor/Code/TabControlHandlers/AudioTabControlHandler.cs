using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public class AudioTabControlHandler : AbstractTabControlHandler
	{
		public override AbstractBinaryFileHandler FileHandler => new ResourceFileHandler(BinaryFileType.Audio);

		public override MenuItem CreateFileTypeMenuItem()
		{
			MenuItem fileTypeMenuItem = base.CreateFileTypeMenuItem();

			MenuItem audioImport = new MenuItem { Header = $"Import Audio paths from folder" };
			MenuItem loudnessImport = new MenuItem { Header = $"Import Loudness from file" };
			MenuItem loudnessExport = new MenuItem { Header = $"Export Loudness to file" };

			audioImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioExpanderControl.Handler.ImportFolder();
			loudnessImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioExpanderControl.Handler.ImportLoudness();
			loudnessExport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioExpanderControl.Handler.ExportLoudness();

			fileTypeMenuItem.Items.Add(audioImport);
			fileTypeMenuItem.Items.Add(new Separator());
			fileTypeMenuItem.Items.Add(loudnessImport);
			fileTypeMenuItem.Items.Add(loudnessExport);

			return fileTypeMenuItem;
		}

		protected override void UpdateExpanderControls(List<GenericUserAsset> assets)
		{
			UpdateExpanderControl(assets.Cast<AudioUserAsset>().ToList(), App.Instance.MainWindow.AudioAudioExpanderControl.Handler);
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.AudioAudioExpanderControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.AudioAudioExpanderControl.Handler.IsComplete();
		}
	}
}