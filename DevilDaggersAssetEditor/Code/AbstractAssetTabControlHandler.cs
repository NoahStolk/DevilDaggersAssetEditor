﻿using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.User;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Code
{
	public abstract class AbstractAssetTabControlHandler<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		protected abstract string AssetTypeJsonFileName { get; }

		public TAsset SelectedAsset { get; set; }
		public List<TAsset> Assets { get; private set; } = new List<TAsset>();

		protected readonly List<TAssetRowControl> assetRowControls = new List<TAssetRowControl>();

		private UserSettings settings => UserHandler.Instance.settings;

		protected AbstractAssetTabControlHandler(BinaryFileType binaryFileType)
		{
			using StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileType.ToString().ToLower()}.{AssetTypeJsonFileName}.json"));
			Assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());
		}

		public abstract void UpdateGui(TAsset asset);

		public void SelectAsset(TAsset asset)
		{
			SelectedAsset = asset;
		}

		public IEnumerable<TAssetRowControl> CreateAssetRowControls()
		{
			int i = 0;
			foreach (TAsset asset in Assets)
			{
				TAssetRowControl assetRowControl = (TAssetRowControl)Activator.CreateInstance(typeof(TAssetRowControl), asset);
				assetRowControl.Background = new SolidColorBrush(Color.FromRgb(asset.ColorR, asset.ColorG, asset.ColorB) * (++i % 2 == 0 ? 0.125f : 0.25f));
				assetRowControls.Add(assetRowControl);
				yield return assetRowControl;
			}
		}

		public void ImportFolder()
		{
			using CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
			if (settings.EnableAssetsRootFolder)
				dialog.InitialDirectory = settings.AssetsRootFolder;

			CommonFileDialogResult result = dialog.ShowDialog();
			if (result != CommonFileDialogResult.Ok)
				return;

			foreach (string filePath in Directory.GetFiles(dialog.FileName))
			{
				TAsset asset = Assets.Where(a => a.AssetName == FileNameToChunkName(Path.GetFileNameWithoutExtension(filePath))).Cast<TAsset>().FirstOrDefault();
				if (asset != null)
				{
					asset.EditorPath = FileNameToChunkName(filePath);
					UpdateGui(asset);
				}
			}
		}

		public bool IsComplete()
		{
			foreach (TAsset asset in Assets)
				if (asset.EditorPath.GetPathValidity() != PathValidity.Valid)
					return false;
			return true;
		}

		public virtual string FileNameToChunkName(string fileName) => fileName;
	}
}