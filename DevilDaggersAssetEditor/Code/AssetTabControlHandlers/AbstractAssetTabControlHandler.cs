﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.User;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Code.AssetTabControlHandlers
{
	public abstract class AbstractAssetTabControlHandler<TAsset, TAssetControl> where TAsset : AbstractAsset where TAssetControl : UserControl
	{
		public List<TAsset> Assets { get; private set; } = new List<TAsset>();

		protected readonly List<TAssetControl> assetControls = new List<TAssetControl>();

		protected abstract string AssetTypeJsonFileName { get; }

		protected AbstractAssetTabControlHandler(BinaryFileType binaryFileType)
		{
			using (StreamReader sr = new StreamReader(DevilDaggersAssetCore.Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileType.ToString().ToLower()}.{AssetTypeJsonFileName}.json")))
				Assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());
		}

		public abstract void UpdateGUI(TAsset asset);

		public IEnumerable<TAssetControl> CreateUserControls()
		{
			int i = 0;
			foreach (TAsset asset in Assets)
			{
				TAssetControl ac = (TAssetControl)Activator.CreateInstance(typeof(TAssetControl), asset);
				ac.Background = new SolidColorBrush(Color.FromRgb(asset.ColorR, asset.ColorG, asset.ColorB) * (++i % 2 == 0 ? 0.125f : 0.25f));
				assetControls.Add(ac);
				yield return ac;
			}
		}

		public void ImportFolder()
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = UserHandler.Instance.settings.AssetsRootFolder })
			{
				CommonFileDialogResult result = dialog.ShowDialog();
				if (result != CommonFileDialogResult.Ok)
					return;

				foreach (string filePath in Directory.GetFiles(dialog.FileName))
				{
					TAsset asset = Assets.Where(a => a.AssetName == FileNameToChunkName(Path.GetFileNameWithoutExtension(filePath))).Cast<TAsset>().FirstOrDefault();
					if (asset != null)
					{
						asset.EditorPath = FileNameToChunkName(filePath);
						UpdateGUI(asset);
					}
				}
			}
		}

		public bool IsComplete()
		{
			foreach (TAsset asset in Assets)
				if (!asset.EditorPath.IsPathValid())
					return false;
			return true;
		}

		public virtual string FileNameToChunkName(string fileName)
		{
			return fileName;
		}
	}
}