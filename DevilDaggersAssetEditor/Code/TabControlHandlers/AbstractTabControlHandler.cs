using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public abstract class AbstractTabControlHandler<TAsset, TAssetControl> where TAsset : AbstractAsset
	{
		public List<TAsset> Assets { get; private set; } = new List<TAsset>();

		protected readonly List<TAssetControl> assetControls = new List<TAssetControl>();

		protected abstract string AssetTypeJsonFileName { get; }

		protected AbstractTabControlHandler(BinaryFileName binaryFileName)
		{
			using (StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileName.ToString().ToLower()}.{AssetTypeJsonFileName}.json")))
				Assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());
		}

		public abstract void UpdatePathLabel(TAsset asset);

		public IEnumerable<TAssetControl> CreateUserControls()
		{
			foreach (TAsset asset in Assets)
			{
				TAssetControl ac = (TAssetControl)Activator.CreateInstance(typeof(TAssetControl), asset);
				assetControls.Add(ac);
				yield return ac;
			}
		}

		public void ImportFolder()
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
			{
				CommonFileDialogResult result = dialog.ShowDialog();
				if (result != CommonFileDialogResult.Ok)
					return;

				foreach (string filePath in Directory.GetFiles(dialog.FileName))
				{
					TAsset asset = Assets.Where(a => a.AssetName == Path.GetFileNameWithoutExtension(filePath)).Cast<TAsset>().FirstOrDefault();
					if (asset != null)
					{
						asset.EditorPath = filePath;
						UpdatePathLabel(asset);
					}
				}
			}
		}

		public bool IsComplete()
		{
			foreach (TAsset asset in Assets)
				if (!Utils.IsPathValid(asset.EditorPath))
					return false;
			return true;
		}
	}
}