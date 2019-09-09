using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code.Assets;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		private static readonly string DDFolder = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		private static readonly Dictionary<AudioAsset, string> assetPaths = new Dictionary<AudioAsset, string>();

		public MainWindow()
		{
			InitializeComponent();

			foreach (BinaryFileName binaryFileName in (BinaryFileName[])Enum.GetValues(typeof(BinaryFileName)))
			{
				MenuItem extractItem = new MenuItem { Header = binaryFileName.ToString() };
				MenuItem compressItem = new MenuItem { Header = binaryFileName.ToString() };

				extractItem.Click += (sender, e) => Extract_Click(binaryFileName);
				compressItem.Click += (sender, e) => Compress_Click(binaryFileName);

				ExtractMenuItem.Items.Add(extractItem);
				CompressMenuItem.Items.Add(compressItem);
			}

			foreach (AudioAsset audioAsset in AssetHandler.Instance.AudioAssets)
			{
				assetPaths[audioAsset] = null;
			}
		}

		private void Extract_Click(BinaryFileName binaryFileName)
		{
			OpenFileDialog dialog = new OpenFileDialog
			{
				InitialDirectory = Path.Combine(DDFolder, binaryFileName.GetSubfolderName())
			};
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value)
			{
				Extractor.Extract(dialog.FileName, Path.Combine("Extracted", binaryFileName.ToString()), binaryFileName);
			}
		}

		private void Compress_Click(BinaryFileName binaryFileName)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				InitialDirectory = Path.Combine(DDFolder, binaryFileName.GetSubfolderName())
			};
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value)
			{
				Compressor.Compress(AssetFolder, dialog.FileName, binaryFileName);
			}
		}

		private void Import_Click(object sender, RoutedEventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true
			})
			{
				CommonFileDialogResult result = dialog.ShowDialog();

				if (result == CommonFileDialogResult.Ok)
				{
					
				}
			}
		}
	}
}