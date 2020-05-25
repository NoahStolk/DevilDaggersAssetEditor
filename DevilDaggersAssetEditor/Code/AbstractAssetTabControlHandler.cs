using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.User;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Code
{
	public abstract class AbstractAssetTabControlHandler<TAsset, TAssetRowControl>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
	{
		protected abstract string AssetTypeJsonFileName { get; }

		public TAsset SelectedAsset { get; set; }
		public List<TAsset> Assets { get; private set; } = new List<TAsset>();

		public readonly List<TAssetRowControl> assetRowControls = new List<TAssetRowControl>();

		private UserSettings settings => UserHandler.Instance.settings;

		public readonly List<StackPanel> filterStackPanels = new List<StackPanel>();
		public readonly List<CheckBox> filterCheckBoxes = new List<CheckBox>();

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
				TAssetRowControl assetRowControl = (TAssetRowControl)Activator.CreateInstance(typeof(TAssetRowControl), asset, i++ % 2 == 0);
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
				TAsset asset = Assets.Where(a => a.AssetName == Path.GetFileNameWithoutExtension(filePath).Replace("_fragment", "").Replace("_vertex", "")).Cast<TAsset>().FirstOrDefault();
				if (asset != null)
				{
					asset.EditorPath = filePath.Replace("_fragment", "").Replace("_vertex", "");
					UpdateGui(asset);
				}
			}
		}

		public bool IsComplete()
		{
			foreach (TAsset asset in Assets)
				if (!File.Exists(asset.EditorPath.Replace(".glsl", "_vertex.glsl")))
					return false;
			return true;
		}

		public void CreateFiltersGui()
		{
			IEnumerable<string> tags = Assets.SelectMany(a => a.Tags ?? (new string[] { })).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(s => s);
			int filterColumnCount = 5;
			int i = 0;
			for (; i < filterColumnCount; i++)
			{
				StackPanel stackPanel = new StackPanel { Tag = i };
				Grid.SetColumn(stackPanel, i);
				filterStackPanels.Add(stackPanel);
			}

			i = 0;
			foreach (string tag in tags)
			{
				int pos = (int)(i++ / (float)tags.Count() * filterColumnCount);
				CheckBox checkBox = new CheckBox { Content = tag };
				filterCheckBoxes.Add(checkBox);
				filterStackPanels.FirstOrDefault(s => (int)s.Tag == pos).Children.Add(checkBox);
			}
		}

		public void ApplyFilter(FilterOperation filterOperation, Dictionary<TAssetRowControl, TAsset> assets)
		{
			IEnumerable<string> checkedFiters = filterCheckBoxes.Where(c => c.IsChecked.Value).Select(s => s.Content.ToString());
			if (checkedFiters.Count() == 0)
			{
				foreach (TAssetRowControl arc in assets.Keys)
					arc.Visibility = Visibility.Visible;
			}
			else
			{
				foreach (KeyValuePair<TAssetRowControl, TAsset> kvp in assets)
				{
					kvp.Key.Visibility = filterOperation switch
					{
						FilterOperation.And => checkedFiters.All(t => kvp.Value.Tags.Contains(t)) ? Visibility.Visible : Visibility.Collapsed,
						FilterOperation.Or => kvp.Value.Tags.Any(t => checkedFiters.Contains(t)) ? Visibility.Visible : Visibility.Collapsed,
						_ => kvp.Key.Visibility,
					};
				}
			}
		}
	}
}