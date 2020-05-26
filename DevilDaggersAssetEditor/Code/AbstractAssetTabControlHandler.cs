using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Info;
using DevilDaggersAssetCore.User;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
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

		public readonly List<TAssetRowControl> assetRowControls = new List<TAssetRowControl>();
		public readonly Dictionary<TAssetRowControl, bool> assetRowControlActiveDict = new Dictionary<TAssetRowControl, bool>();

		private UserSettings settings => UserHandler.Instance.settings;

		public readonly List<StackPanel> filterStackPanels = new List<StackPanel>();
		public readonly List<CheckBox> filterCheckBoxes = new List<CheckBox>();

		private readonly Color filterHighlightColor;

		protected AbstractAssetTabControlHandler(BinaryFileType binaryFileType)
		{
			using StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileType.ToString().ToLower()}.{AssetTypeJsonFileName}.json"));
			Assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == typeof(TAsset));
			filterHighlightColor = chunkInfo.GetColor() * 0.25f;
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
				assetRowControlActiveDict.Add(assetRowControl, true);
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
			int filterColumnCount = 9;
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

		public void ApplyFilter(FilterOperation filterOperation, Dictionary<TAssetRowControl, TAsset> assets, Dictionary<TAssetRowControl, TextBlock> textBlocks)
		{
			IEnumerable<string> checkedFiters = filterCheckBoxes.Where(c => c.IsChecked.Value).Select(s => s.Content.ToString());

			foreach (KeyValuePair<TAssetRowControl, TAsset> kvp in assets)
			{
				TextBlock textBlockTags = textBlocks.FirstOrDefault(t => t.Key == kvp.Key).Value;

				if (checkedFiters.Count() == 0)
				{
					assetRowControlActiveDict[kvp.Key] = true;

					textBlockTags.Text = string.Join(", ", kvp.Value.Tags);
				}
				else
				{
					assetRowControlActiveDict[kvp.Key] = filterOperation switch
					{
						FilterOperation.And => checkedFiters.All(t => kvp.Value.Tags.Contains(t)),
						FilterOperation.Or => kvp.Value.Tags.Any(t => checkedFiters.Contains(t)),
						_ => throw new NotImplementedException($"{nameof(FilterOperation)} {filterOperation} not implemented in {nameof(ApplyFilter)} method.")
					};
					if (!assetRowControlActiveDict[kvp.Key])
						continue;

					// If not collapsed, change TextBlock colors for found tags.
					textBlockTags.Inlines.Clear();

					string[] tags = Assets.FirstOrDefault(a => a == kvp.Value).Tags;
					for (int i = 0; i < tags.Length; i++)
					{
						string tag = tags[i];
						Run tagRun = new Run(tag);
						if (checkedFiters.Contains(tag))
							tagRun.Background = new SolidColorBrush(filterHighlightColor);
						textBlockTags.Inlines.Add(tagRun);
						if (i != tags.Length - 1)
							textBlockTags.Inlines.Add(new Run(", "));
					}
				}
			}
		}
	}
}