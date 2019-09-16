using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public abstract class AbstractTabControlHandler<TAsset, TAssetControl> where TAsset : AbstractAsset
	{
		private readonly BinaryFileName binaryFileName;

		public List<TAsset> Assets { get; private set; } = new List<TAsset>();

		protected readonly List<TAssetControl> assetControls = new List<TAssetControl>();

		protected abstract string AssetTypeJsonFileName { get; }

		protected AbstractTabControlHandler(BinaryFileName binaryFileName)
		{
			this.binaryFileName = binaryFileName;

			using (StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileName.ToString().ToLower()}.{AssetTypeJsonFileName}.json")))
				Assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());
		}

		protected abstract void UpdatePathLabel(TAsset asset);

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

		public void Compress()
		{
			bool complete = true;
			foreach (TAsset asset in Assets)
			{
				if (!Utils.IsPathValid(asset.EditorPath))
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
				InitialDirectory = Path.Combine(Utils.DDFolder, binaryFileName.GetSubfolderName())
			};
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value)
				return;

			Compressor.Compress(Assets.Cast<AbstractAsset>().ToList(), dialog.FileName, binaryFileName);
		}
	}
}