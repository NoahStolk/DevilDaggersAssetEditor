﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.BinaryFileHandlers;
using DevilDaggersAssetCore.ModFiles;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code.FileTabControlHandlers
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

			audioImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioAssetTabControl.Handler.ImportFolder();
			loudnessImport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioAssetTabControl.Handler.ImportLoudness();
			loudnessExport.Click += (sender, e) => App.Instance.MainWindow.AudioAudioAssetTabControl.Handler.ExportLoudness();

			fileTypeMenuItem.Items.Add(audioImport);
			fileTypeMenuItem.Items.Add(new Separator());
			fileTypeMenuItem.Items.Add(loudnessImport);
			fileTypeMenuItem.Items.Add(loudnessExport);

			return fileTypeMenuItem;
		}

		protected override void UpdateAssetTabControls(List<AbstractUserAsset> assets)
		{
			UpdateAssetTabControl(assets.Cast<AudioUserAsset>().ToList(), App.Instance.MainWindow.AudioAudioAssetTabControl.Handler);
		}

		public override List<AbstractAsset> GetAssets()
		{
			return App.Instance.MainWindow.AudioAudioAssetTabControl.Handler.Assets.Cast<AbstractAsset>().ToList();
		}

		protected override bool IsComplete()
		{
			return App.Instance.MainWindow.AudioAudioAssetTabControl.Handler.IsComplete();
		}
	}
}