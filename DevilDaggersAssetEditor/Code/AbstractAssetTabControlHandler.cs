using DevilDaggersAssetCore;
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

namespace DevilDaggersAssetEditor.Code
{
	internal abstract class AbstractAssetTabControlHandler<TAsset, TAssetControl>
		where TAsset : AbstractAsset
		where TAssetControl : UserControl
	{
		internal TAsset SelectedAsset { get; set; }

		internal List<TAsset> Assets { get; private set; } = new List<TAsset>();

		private protected readonly List<TAssetControl> assetControls = new List<TAssetControl>();

		private protected abstract string AssetTypeJsonFileName { get; }

		protected AbstractAssetTabControlHandler(BinaryFileType binaryFileType)
		{
			using (StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileType.ToString().ToLower()}.{AssetTypeJsonFileName}.json")))
				Assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());
		}

		internal abstract void UpdateGui(TAsset asset);

		internal void SelectAsset(TAsset asset)
		{
			SelectedAsset = asset;
		}

		internal IEnumerable<TAssetControl> CreateAssetControls()
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

		internal void ImportFolder()
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
						UpdateGui(asset);
					}
				}
			}
		}

		internal bool IsComplete()
		{
			foreach (TAsset asset in Assets)
				if (!asset.EditorPath.IsPathValid())
					return false;
			return true;
		}

		internal virtual string FileNameToChunkName(string fileName) => fileName;
	}
}